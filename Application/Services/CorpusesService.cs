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
using Application.Dtos.Collocations;
using Application.Dtos.Words;

namespace Application.Services
{
    //this class realizes operations - reading, searching etc. on corpuses
    public class CorpusesService : ICorpusesService
    {
        private readonly IArchiveService _archivesService;
        private readonly ICorpusesRepository _corpusesRepository;
        private readonly IClarinService _clarinService;
        private readonly ICacheRepository _cacheRepository;
        private readonly ISearchCorpusService _searchCorpusService;
        private readonly IMapper _mapper;

        public CorpusesService(IClarinService clarinService, ISearchCorpusService searchCorpusService, ICorpusesRepository corpusesRepository,
                                IArchiveService archivesService, IMapper mapper, ICacheRepository cacheRepository)
        {
            _archivesService = archivesService;
            _corpusesRepository = corpusesRepository;
            _clarinService = clarinService;
            _searchCorpusService = searchCorpusService;
            _mapper = mapper;
            _cacheRepository = cacheRepository;
        }

        public async Task<CorpusDto> CreateFromZIP_Async(IFormFile zipFile)
        {
            var zipArchive = _archivesService.GetZipArchiveFromIFormFile(zipFile);
            if (zipArchive == null)
                return null;

            CorpusDto corpusDto = new CorpusDto();
            var CCLs = new List<string>();
            foreach (var e in zipArchive.Entries)
            {
                var ccl = await _clarinService.GetCCLStringFromZipArchiveEntry(e);
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
            return await Task.FromResult(xd != null ? xd.Collocations: null);
        }

        public async Task<List<CollocationsMetaData>> GetCollocationsBySentence_Async(Guid corpusId, string word, int direction)
        {
            var collocations = _cacheRepository.GetCollocationsBySentence(corpusId, word, direction);
            if (collocations != null)
                return await collocations;

            var xd = await _searchCorpusService.GetCollocationsBySentenceAsync(corpusId, word, direction);

            _cacheRepository.InsertIntoCache<CollocationsInfo>(corpusId, word, xd);
            return await Task.FromResult(xd != null ? xd.CollocationsBySentence: null);
        }

        public async Task<List<CollocationsMetaData>> GetCollocationsByParagraph_Async(Guid corpusId, string word, int direction)
        {
            var collocationsByParagraph = _cacheRepository.GetCollocationsByParagraph(corpusId, word, direction);
            if (collocationsByParagraph != null)
                return await collocationsByParagraph;

            var xd = await _searchCorpusService.GetCollocationsByParagraphAsync(corpusId, word, direction);

            _cacheRepository.InsertIntoCache<CollocationsInfo>(corpusId, word, xd);
            return await Task.FromResult(xd != null ? xd.CollocationsByParagraph: null);
        }

        public async Task<int> GetWordAppearance_Async(Guid corpusId, string word)
        {
            var apperances = _cacheRepository.GetWordAppearance(corpusId, word);
            if (apperances != null)
                return await apperances;

            var xd = await _searchCorpusService.GetApperancesWithFilenamesAsync(corpusId, word);

            _cacheRepository.InsertIntoCache<WordInfo>(corpusId, word, xd);
            return await Task.FromResult(xd != null ? xd.WordCountInCorpus: 0);
        }

        public async Task<Dictionary<string, int>> GetWordAppearanceWithFileNames_Async(Guid corpusId, string word)
        {
            var apperancesWithFilenames = _cacheRepository.GetWordAppearanceWithFilenames(corpusId, word);
            if (apperancesWithFilenames != null)
                return await apperancesWithFilenames;

            var xd = await _searchCorpusService.GetApperancesWithFilenamesAsync(corpusId, word);

            _cacheRepository.InsertIntoCache<WordInfo>(corpusId, word, xd);
            return await Task.FromResult(xd != null ? xd.FilenameWithWordCountDict: null);
        }
    }
}