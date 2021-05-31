using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Cache;
using Application.Dtos;
using Application.Dtos.Collocations;
using Application.Dtos.Temporary;
using Application.Dtos.Words;
using Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Application.Repositories
{
    public class CacheRepository : ICacheRepository
    {
        private MemoryCache _cache;
        
        public CacheRepository(CorpusesCache corpusesCache)
        {
            _cache = corpusesCache.Cache;
        }

        public Task<List<TokenDto>> GetCollocations(Guid corpusId, string word, int direction)
        {
            var collocationsElement = _cache.Get(corpusId.ToString() + "|" + word) as CollocationsInfo;
            if (collocationsElement != null && collocationsElement.Direction == direction)
                return Task.FromResult(collocationsElement.Collocations);
            return null;
        }

        public Task<List<CollocationsMetaData>> GetCollocationsByParagraph(Guid corpusId, string word, int direction)
        {
            var element = _cache.Get(corpusId.ToString() + "|" + word) as CollocationsInfo;
            if (element != null)
            {
                if(element.Direction == direction)
                {
                    int innerCount = 0;
                    element.CollocationsByParagraph.ForEach(c => innerCount += c.Collocations.Count);
                    if(element.Collocations.Count == innerCount)
                        return Task.FromResult(element.CollocationsByParagraph);
                }
            }            
            return null;
        }

        public Task<List<CollocationsMetaData>> GetCollocationsBySentence(Guid corpusId, string word, int direction)
        {
            var element = _cache.Get(corpusId.ToString() + "|" + word) as CollocationsInfo;
            if (element != null)
            {
                if(element.Direction == direction)
                {
                    int innerCount = 0;
                    element.CollocationsBySentence.ForEach(c => innerCount += c.Collocations.Count);
                    if(element.Collocations.Count == innerCount)
                        return Task.FromResult(element.CollocationsBySentence);
                }
            }            
            return null;
        }

        public Task<CollocationsInfo> GetCollocationsInfoElement(Guid corpusId, string word)
        {
            var element = _cache.Get(corpusId.ToString() + "|" + word) as CollocationsInfo;
            if (element != null)
                return Task.FromResult(element);
            return null;
        }

        public Task<int> GetWordAppearance(Guid corpusId, string word)
        {
            var element = _cache.Get(corpusId.ToString() + "|" + word) as WordInfo;
            if (element != null)
                return Task.FromResult(element.WordCountInCorpus);
            return null;
        }

        public Task<Dictionary<string, int>> GetWordAppearanceWithFilenames(Guid corpusId, string word)
        {
            var element = _cache.Get(corpusId.ToString() + "|" + word) as WordInfo;
            if (element != null)
                return Task.FromResult(element.FilenameWithWordCountDict);
            return null;
        }

        public Task<WordInfo> GetWordInfoFromCache(Guid corpusId, string word)
        {
            var element = _cache.Get(corpusId.ToString() + "|" + word) as WordInfo;
            if (element != null)
                return Task.FromResult(element);
            return null;
        }

        public void InsertIntoCache<T>(Guid corpusId, string word, T element)
        {
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSize(1)
                .SetSlidingExpiration(TimeSpan.FromMinutes(30));
            _cache.Set<T>(corpusId.ToString() + "|" + word, element, cacheOptions);
        }
    }
}