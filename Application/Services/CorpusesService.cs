using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Application.Dtos;
using Application.Dtos.Clarin;
using Application.Dtos.Temporary;
using Application.Interfaces;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Application.Cache;

namespace Application.Services
{
    //this class realizes operations - reading, searching etc. on corpuses
    public class CorpusesService : ICorpusesService
    {
        public CorpusMetaDataDto CorpusMetaData { get; set; }

        private readonly IClarinService _clarinService;
        private readonly ICorpusesRepository _corpusesRepository;
        private readonly IMapper _mapper;

        public CorpusesService(IClarinService clarinService, ICorpusesRepository corpusesRepository,
                                IMapper mapper)
        {
            _clarinService = clarinService;
            _corpusesRepository = corpusesRepository;
            _mapper = mapper;
        }

        public ChunkListDto ParseCCLStringToChunkListDto(string ccl)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ChunkListDto));
            ChunkListDto result;
            using (StringReader reader = new StringReader(ccl))
            {
                result = (ChunkListDto)serializer.Deserialize(reader);
            }

            return result;
        }

        public async Task<CorpusDto> CreateFromZIP_Async(IFormFile zipFile)
        {
            CorpusDto corpusDto = new CorpusDto();
            try
            {
                var archive = new ZipArchive(zipFile.OpenReadStream(), ZipArchiveMode.Read);
                var CCLs = new List<string>();
                foreach (var e in archive.Entries)
                {
                    var ccl = await ParseToCCL_Async(e);
                    CCLs.Add(ccl);
                    var chunkListDto = ParseCCLStringToChunkListDto(ccl);
                    chunkListDto._chunkListMetaData.OriginFileName = e.Name;
                    corpusDto.ChunkLists.Add(chunkListDto);
                }

                corpusDto.CorpusMetaData = new CorpusMetaDataDto(corpusDto, zipFile.FileName, "anybody");

                // database changes
                Corpus corpus = _mapper.Map<CorpusDto, Corpus>(corpusDto);
                _corpusesRepository.CreateCorpus(corpus);
                _corpusesRepository.SaveChanges();
                corpusDto.Id = corpus.Id;

                return corpusDto;
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException || ex is InvalidDataException)
                    return null;
                throw;
            }
        }

        public async Task<string> ParseToCCL_Async(ZipArchiveEntry entry)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                entry.Open().CopyTo(ms);

                var fileId = await _clarinService.UploadFile_ApiPostAsync(ms.ToArray());
                var taskId = await _clarinService.UseWCRFT2Tager_ApiPostAsync(fileId);

                TaskStatusDto taskStatus;
                string ccl = "";
                do
                {
                    taskStatus = await _clarinService.GetTaskStatus_ApiGetAsync(taskId);
                    if (taskStatus.Status == "ERROR")
                    {
                        ccl = $"CLARIN ERROR WHILE PARSING|{entry.Name}";
                        break;
                    }
                    else if (taskStatus.UnknowStatus)
                    {
                        ccl = $"CLARIN UNKNOW STATUS WHILE PARSING:|{entry.Name}";
                        break;
                    }
                    else if (taskStatus.Status != "DONE")
                    {
                        //czekamy pół sekundy może się coś zmieni
                        await Task.Delay(500);
                    }

                } while (taskStatus.Status != "DONE");

                if (taskStatus.Status != "ERROR")
                    ccl = await _clarinService.DownloadCompletedTask_ApiGetAsync(taskStatus.ResultFileId);

                return ccl;
            }
        }

        public Task<List<TokenDto>> GetCollocations_Async(Guid corpusId, string word, int distance)
        {
            var collocationsModel = _GetCollocationModelFromCache(corpusId, word);
            if(collocationsModel != null && collocationsModel.Collocations != null)
                return Task.FromResult(collocationsModel.Collocations);
            collocationsModel = new CollocationsModel(corpusId, word /*options like distance, grouping by lexem etc.*/);    

            var corpusChunkListMetaDatas = _corpusesRepository.GetChunkListMetaDatasByCorpusId(corpusId);

            var collocations = new List<TokenDto>();
            int count = 0;
            foreach (var c in corpusChunkListMetaDatas)
            {
                var chunksDtos = _GetChunkDtos(c, word);

                int countWithFilenames = 0;
                foreach (var chunkDtoWithWord in chunksDtos)
                {
                    foreach (var sentence in chunkDtoWithWord.Sentences)
                    {
                        List<TokenDto> tokens = sentence.Tokens.ToList();
                        if (distance < 0)
                            tokens.Reverse();
                        do
                        {
                            tokens = tokens.SkipWhile(t => !t.Orth.ToLower().Equals(word.ToLower())).Skip(Math.Abs(distance)).ToList();
                            var token = tokens.FirstOrDefault();
                            if (token != null && !collocations.Contains(token))
                                count++;
                                countWithFilenames++;
                                collocations.Add(token);
                            tokens = tokens.Skip(1).ToList();
                        } while (tokens.Any());
                    }
                }
                collocationsModel.WordApperancesWithFilenames.Add(new Tuple<int, string>(countWithFilenames, c.OriginFileName));
            }
            collocationsModel.WordCountInCorpus = count;
            collocationsModel.Collocations = collocations;
            //TODO put collocationsModel in cache

            return Task.FromResult(collocations);
        }

        //TODO
        public Task<List<List<TokenDto>>> GetCollocationsBySentence_Async(Guid corpusId, string word, int distance)
        {
            throw new NotImplementedException();
        }

        //TODO
        public Task<List<List<TokenDto>>> GetCollocationsByParagraph_Async(Guid corpusId, string word, int distance)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetWordAppearance_Async(Guid corpusId, string word)
        {
            var collocationsModel = _GetCollocationModelFromCache(corpusId, word);
            if(collocationsModel != null)
                return Task.FromResult(collocationsModel.WordCountInCorpus);
            collocationsModel = new CollocationsModel(corpusId, word /*options like distance, grouping by lexem etc.*/);

            var corpusChunkListMetaDatas = _corpusesRepository.GetChunkListMetaDatasByCorpusId(corpusId);

            int count = 0;
            var apperancesWithFilenames = new List<Tuple<int, string>>();
            foreach (var c in corpusChunkListMetaDatas)
            {
                var chunksDtos = _GetChunkDtos(c, word);

                int countWithFilenames = 0;
                foreach (var chunkDtoWithWord in chunksDtos)
                {
                    foreach (var sentence in chunkDtoWithWord.Sentences)
                    {
                        foreach (var token in sentence.Tokens.Select((value, i) => (value, i)))
                        {
                            if (token.value.Orth.ToLower() == word.ToLower())
                            {
                                count++;
                                countWithFilenames++;
                            }
                                
                        }
                    }
                }
                apperancesWithFilenames.Add(new Tuple<int, string>(countWithFilenames, c.OriginFileName));
            }

            collocationsModel.WordApperancesWithFilenames = apperancesWithFilenames;
            collocationsModel.WordCountInCorpus = count;
            //TODO put collocationsModel in cache

            return Task.FromResult(count);
        }

        public Task<List<Tuple<int, string>>> GetWordAppearanceWithFileNames_Async(Guid corpusId, string word)
        {
            var collocationsModel = _GetCollocationModelFromCache(corpusId, word);
            if(collocationsModel != null && collocationsModel.WordApperancesWithFilenames != null)
                return Task.FromResult(collocationsModel.WordApperancesWithFilenames);
            collocationsModel = new CollocationsModel(corpusId, word /*options like distance, grouping by lexem etc.*/);

            var corpusChunkListMetaDatas = _corpusesRepository.GetChunkListMetaDatasByCorpusId(corpusId);

            var apperancesWithFilenames = new List<Tuple<int, string>>();
            int count = 0;
            foreach (var c in corpusChunkListMetaDatas)
            {
                var chunksDtos = _GetChunkDtos(c, word);

                int countWithFilenames = 0;
                foreach (var chunkDtoWithWord in chunksDtos)
                {
                    foreach (var sentence in chunkDtoWithWord.Sentences)
                    {
                        var innerCount = sentence.Tokens.Where(t => t.Orth.ToLower().Equals(word.ToLower())).Count();
                        countWithFilenames += innerCount;
                        count += innerCount;
                    }
                         
                }
                apperancesWithFilenames.Add(new Tuple<int, string>(countWithFilenames, c.OriginFileName));
            }

            collocationsModel.WordApperancesWithFilenames = apperancesWithFilenames;
            collocationsModel.WordCountInCorpus = count;
            //TODO put collocationsModel in cache 

            return Task.FromResult(apperancesWithFilenames);
        }

        private List<ChunkDto> _GetChunkDtos(ChunkListMetaData c, string word)
        {
            var chunkListMetaData = _mapper.Map<ChunkListMetaDataDto>(c);
            if (!chunkListMetaData.WordsLookupDictionary.ContainsKey(word))
                return null;
            List<int> idsOfChunksWithWord = chunkListMetaData.WordsLookupDictionary[word];
            var chunks = _corpusesRepository.GetChunksByChunkListIdAndXmlChunkId(c.ChunkList.Id, idsOfChunksWithWord);
            return _mapper.Map<List<ChunkDto>>(chunks);
        }

        //TODO check if collocationsModel exisits in cache
        private CollocationsModel _GetCollocationModelFromCache(Guid corpusId, string word)
        {
            return null;
        }
    }
}