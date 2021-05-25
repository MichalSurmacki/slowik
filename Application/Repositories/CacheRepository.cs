using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Cache;
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

        public Task<List<TokenDto>> GetCollocations(Guid corpusId, string word)
        {
            var collocationsElement = _cache.Get(corpusId.ToString() + "|" + word) as CacheCollocationsInfoElement;
            if (collocationsElement != null)
                return Task.FromResult(collocationsElement.Collocations);
            return null;
        }

        public Task<List<List<TokenDto>>> GetCollocationsByParagraph(Guid corpusId, string word)
        {
            var element = _cache.Get(corpusId.ToString() + "|" + word) as CacheCollocationsInfoElement;
            if (element != null)
            {
                int count = 0;
                element.CollocationsByParagraph.ForEach(l => count += l.Count);
                if(element.Collocations.Count.Equals(count))
                    return Task.FromResult(element.CollocationsByParagraph);
            }            
            return null;
        }

        public Task<List<List<TokenDto>>> GetCollocationsBySentence(Guid corpusId, string word)
        {
            var element = _cache.Get(corpusId.ToString() + "|" + word) as CacheCollocationsInfoElement;
            if (element != null)
            {
                int count = 0;
                element.CollocationsBySentence.ForEach(l => count += l.Count);
                if(element.Collocations.Count.Equals(count))
                    return Task.FromResult(element.CollocationsBySentence);
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