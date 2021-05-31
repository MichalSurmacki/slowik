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
using Application.Dtos.Collocations;
using Application.Dtos.Words;

namespace Application.Services
{
    //this class realizes operations - reading, searching etc. on corpuses
    public class CorpusesService : ICorpusesService
    {
        private readonly ICorpusesRepository _corpusesRepository;
        private readonly IClarinService _clarinService;
        private readonly ICacheRepository _cacheRepository;
        private readonly ISearchCorpusService _searchCorpusService;
        private readonly IMapper _mapper;

        public CorpusesService(IClarinService clarinService, ISearchCorpusService searchCorpusService, ICorpusesRepository corpusesRepository,
                                IMapper mapper, ICacheRepository cacheRepository)
        {
            _corpusesRepository = corpusesRepository;
            _clarinService = clarinService;
            _searchCorpusService = searchCorpusService;
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

        public async Task<List<TokenDto>> GetCollocations_Async(Guid corpusId, string word, int direction)
        {
            var collocations = _cacheRepository.GetCollocations(corpusId, word, direction);
            if (collocations != null)
                return await collocations;

            var xd = await _searchCorpusService.GetAllCollocationsAsync(corpusId, word, direction);

            _cacheRepository.InsertIntoCache<CollocationsInfo>(corpusId, word, xd);
            return await Task.FromResult(xd.Collocations);
        }

        public async Task<List<CollocationsMetaData>> GetCollocationsBySentence_Async(Guid corpusId, string word, int direction)
        {
            var collocations = _cacheRepository.GetCollocationsBySentence(corpusId, word, direction);
            if (collocations != null)
                return await collocations;

            var xd = await _searchCorpusService.GetCollocationsBySentenceAsync(corpusId, word, direction);

            _cacheRepository.InsertIntoCache<CollocationsInfo>(corpusId, word, xd);
            return await Task.FromResult(xd.CollocationsBySentence);
        }

        public async Task<List<CollocationsMetaData>> GetCollocationsByParagraph_Async(Guid corpusId, string word, int direction)
        {
            var collocationsByParagraph = _cacheRepository.GetCollocationsByParagraph(corpusId, word, direction);
            if (collocationsByParagraph != null)
                return await collocationsByParagraph;

            var xd = await _searchCorpusService.GetCollocationsByParagraphAsync(corpusId, word, direction);

            _cacheRepository.InsertIntoCache<CollocationsInfo>(corpusId, word, xd);
            return await Task.FromResult(xd.CollocationsByParagraph);;
        }

        public async Task<int> GetWordAppearance_Async(Guid corpusId, string word)
        {
            var apperances = _cacheRepository.GetWordAppearance(corpusId, word);
            if (apperances != null)
                return await apperances;

            var xd = await _searchCorpusService.GetApperancesWithFilenamesAsync(corpusId, word);

            _cacheRepository.InsertIntoCache<WordInfo>(corpusId, word, xd);
            return await Task.FromResult(xd.WordCountInCorpus);
        }

        public async Task<Dictionary<string, int>> GetWordAppearanceWithFileNames_Async(Guid corpusId, string word)
        {
            var apperancesWithFilenames = _cacheRepository.GetWordAppearanceWithFilenames(corpusId, word);
            if (apperancesWithFilenames != null)
                return await apperancesWithFilenames;

            var xd = await _searchCorpusService.GetApperancesWithFilenamesAsync(corpusId, word);

            _cacheRepository.InsertIntoCache<WordInfo>(corpusId, word, xd);
            return await Task.FromResult(xd.FilenameWithWordCountDict);
        }
    }
}