using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Cache;
using Application.Dtos;
using Application.Dtos.Temporary;
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
            var collocationsElement = _cache.Get(corpusId.ToString() + "|" + word) as CacheCollocationsInfoElement;
            if (collocationsElement != null && collocationsElement.Direction == direction)
                return Task.FromResult(collocationsElement.GetCollocations());
            return null;
        }

        public Task<Dictionary<string, List<TokenDto>>> GetCollocationsByParagraph(Guid corpusId, string word, int direction)
        {
            var element = _cache.Get(corpusId.ToString() + "|" + word) as CacheCollocationsInfoElement;
            if (element != null)
            {
                int count = 0;
                foreach(KeyValuePair<string, List<TokenDto>> e in element.GetCollocationsByParagraph())
                    count += e.Value.Count;
                if(element.GetCollocations().Count.Equals(count))
                    return Task.FromResult(element.GetCollocationsByParagraph());
            }            
            return null;
        }

        public Task<Dictionary<string, List<TokenDto>>> GetCollocationsBySentence(Guid corpusId, string word, int direction)
        {
            var element = _cache.Get(corpusId.ToString() + "|" + word) as CacheCollocationsInfoElement;
            if (element != null)
            {
                int count = 0;
                foreach(KeyValuePair<string, List<TokenDto>> e in element.GetCollocationsBySentence())
                    count += e.Value.Count;
                if(element.GetCollocations().Count.Equals(count))
                    return Task.FromResult(element.GetCollocationsBySentence());
            }            
            return null;
        }

        public Task<CacheCollocationsInfoElement> GetCollocationsInfoElement(Guid corpusId, string word)
        {
            var element = _cache.Get(corpusId.ToString() + "|" + word) as CacheCollocationsInfoElement;
            if (element != null)
                return Task.FromResult(element);
            return null;
        }

        public Task<int> GetWordAppearance(Guid corpusId, string word)
        {
            var element = _cache.Get(corpusId.ToString() + "|" + word) as CacheWordInfoElement;
            if (element != null)
                return Task.FromResult(element.WordCountInCorpus);
            return null;
        }

        public Task<Dictionary<string, int>> GetWordAppearanceWithFilenames(Guid corpusId, string word)
        {
            var element = _cache.Get(corpusId.ToString() + "|" + word) as CacheWordInfoElement;
            if (element != null)
                return Task.FromResult(element.GetApperanceInFilesDict());
            return null;
        }

        public Task<CacheWordInfoElement> GetWordInfoFromCache(Guid corpusId, string word)
        {
            var element = _cache.Get(corpusId.ToString() + "|" + word) as CacheWordInfoElement;
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