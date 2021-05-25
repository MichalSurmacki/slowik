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
using Microsoft.Extensions.Caching.Memory;

namespace Application.Services
{
    //this class realizes operations - reading, searching etc. on corpuses
    public class CorpusesService : ICorpusesService
    {
        private readonly IClarinService _clarinService;
        private readonly ICorpusesRepository _corpusesRepository;
        private readonly ICacheRepository _cacheRepository;
        private readonly IMapper _mapper;

        public CorpusesService(IClarinService clarinService, ICorpusesRepository corpusesRepository,
                                IMapper mapper, ICacheRepository cacheRepository)
        {
            _clarinService = clarinService;
            _corpusesRepository = corpusesRepository;
            _mapper = mapper;
            _cacheRepository = cacheRepository;
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

        public Task<List<TokenDto>> GetCollocations_Async(Guid corpusId, string word, int distance)
        {
            var collocations = _cacheRepository.GetCollocations(corpusId, word);
            if (collocations != null)
                return collocations;

            CacheCollocationsInfoElement collocationsInfo = new CacheCollocationsInfoElement(corpusId, word);

            var corpusChunkListMetaDatas = _corpusesRepository.GetChunkListMetaDatasByCorpusId(corpusId);

            foreach (var c in corpusChunkListMetaDatas)
            {
                var chunksDtos = _GetChunkDtos(c, word);
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
                            if (token != null)
                            {
                                collocationsInfo.WordCountInCorpus++;
                                collocationsInfo.AddApperanceInFilename(c.OriginFileName);
                                if(!collocationsInfo.Collocations.Contains(token))                            
                                    collocationsInfo.Collocations.Add(token);
                            }
                            tokens = tokens.Skip(1).ToList();
                        } while (tokens.Any());
                    }
                }
            }
            
            _cacheRepository.InsertIntoCache<CacheCollocationsInfoElement>(corpusId, word, collocationsInfo);
            return Task.FromResult(collocationsInfo.Collocations);
        }

        public Task<int> GetWordAppearance_Async(Guid corpusId, string word)
        {
            var apperances = _cacheRepository.GetWordAppearance(corpusId, word);
            if (apperances != null)
                return apperances;

            CacheWordInfoElement wordInfo = new CacheWordInfoElement(corpusId, word);

            var corpusChunkListMetaDatas = _corpusesRepository.GetChunkListMetaDatasByCorpusId(corpusId);

            foreach (var c in corpusChunkListMetaDatas)
            {
                var chunksDtos = _GetChunkDtos(c, word);
                foreach (var chunkDtoWithWord in chunksDtos)
                {
                    foreach (var sentence in chunkDtoWithWord.Sentences)
                    {
                        int innerCount = sentence.Tokens.Where(t => t.Orth.ToLower().Equals(word.ToLower())).Count();
                        wordInfo.WordCountInCorpus += innerCount;
                        wordInfo.AddApperanceInFilename(c.OriginFileName, innerCount);
                    }
                }
            }

            _cacheRepository.InsertIntoCache<CacheWordInfoElement>(corpusId, word, wordInfo);
            return Task.FromResult(wordInfo.WordCountInCorpus);
        }

        public Task<Dictionary<string, int>> GetWordAppearanceWithFileNames_Async(Guid corpusId, string word)
        {
            var apperancesWithFilenames = _cacheRepository.GetWordAppearanceWithFilenames(corpusId, word);
            if (apperancesWithFilenames != null)
                return apperancesWithFilenames;

            CacheWordInfoElement wordInfo = new CacheWordInfoElement(corpusId, word);

            var corpusChunkListMetaDatas = _corpusesRepository.GetChunkListMetaDatasByCorpusId(corpusId);

            foreach (var c in corpusChunkListMetaDatas)
            {
                var chunksDtos = _GetChunkDtos(c, word);
                foreach (var chunkDtoWithWord in chunksDtos)
                {
                    foreach (var sentence in chunkDtoWithWord.Sentences)
                    {
                        int innerCount = sentence.Tokens.Where(t => t.Orth.ToLower().Equals(word.ToLower())).Count();
                        if (innerCount > 0)
                        {
                            wordInfo.WordCountInCorpus += innerCount;
                            wordInfo.AddApperanceInFilename(c.OriginFileName, innerCount);
                        }
                    }
                }
            }

            _cacheRepository.InsertIntoCache<CacheWordInfoElement>(corpusId, word, wordInfo);
            return Task.FromResult(wordInfo.GetApperanceInFilesDict());
        }

        public Task<List<List<TokenDto>>> GetCollocationsBySentence_Async(Guid corpusId, string word, int distance)
        {
            throw new NotImplementedException();
        }

        public Task<List<List<TokenDto>>> GetCollocationsByParagraph_Async(Guid corpusId, string word, int distance)
        {
            var collocationsByParagraph = _cacheRepository.GetCollocationsByParagraph(corpusId, word);
            if (collocationsByParagraph != null)
                return collocationsByParagraph;

            CacheCollocationsInfoElement collocationsInfo = new CacheCollocationsInfoElement(corpusId, word);

            var corpusChunkListMetaDatas = _corpusesRepository.GetChunkListMetaDatasByCorpusId(corpusId);

            foreach (var c in corpusChunkListMetaDatas)
            {
                var chunksDtos = _GetChunkDtos(c, word);
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
                            if (token != null && !collocationsInfo.Collocations.Contains(token))
                            {
                                collocationsInfo.WordCountInCorpus++;
                                collocationsInfo.AddApperanceInFilename(c.OriginFileName);
                                collocationsInfo.Collocations.Add(token);
                            }
                            tokens = tokens.Skip(1).ToList();
                        } while (tokens.Any());
                    }
                }
            }


            throw new NotImplementedException();
        }

        private List<ChunkDto> _GetChunkDtos(ChunkListMetaData c, string word)
        {
            var chunkListMetaData = _mapper.Map<ChunkListMetaDataDto>(c);
            if (!chunkListMetaData.WordsLookupDictionary.ContainsKey(word))
                return new List<ChunkDto>();
            List<int> idsOfChunksWithWord = chunkListMetaData.WordsLookupDictionary[word];
            var chunks = _corpusesRepository.GetChunksByChunkListIdAndXmlChunkId(c.ChunkList.Id, idsOfChunksWithWord);
            return _mapper.Map<List<ChunkDto>>(chunks);
        }
    }
}