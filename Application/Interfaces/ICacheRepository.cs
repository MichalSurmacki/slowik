using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Cache;
using Application.Dtos.Temporary;

namespace Application.Interfaces
{
    public interface ICacheRepository
    {
        void InsertIntoCache<T>(Guid corpusId, string word, T element);

        Task<CacheWordInfoElement> GetWordInfoFromCache(Guid corpusId, string word);
        Task<int> GetWordAppearance(Guid corpusId, string word);
        Task<Dictionary<string, int>> GetWordAppearanceWithFilenames(Guid corpusId, string word);
        
        Task<CacheCollocationsInfoElement> GetCollocationsInfoElement(Guid corpusId, string word);
        Task<List<TokenDto>> GetCollocations(Guid corpusId, string word);
        Task<List<List<TokenDto>>> GetCollocationsByParagraph(Guid corpusId, string word);
        Task<List<List<TokenDto>>> GetCollocationsBySentence(Guid corpusId, string word);
    }
}