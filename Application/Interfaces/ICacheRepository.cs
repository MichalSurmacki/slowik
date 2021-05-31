using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Cache;
using Application.Dtos;
using Application.Dtos.Collocations;
using Application.Dtos.Temporary;
using Application.Dtos.Words;

namespace Application.Interfaces
{
    public interface ICacheRepository
    {
        void InsertIntoCache<T>(Guid corpusId, string word, T element);

        Task<WordInfo> GetWordInfoFromCache(Guid corpusId, string word);
        Task<int> GetWordAppearance(Guid corpusId, string word);
        Task<Dictionary<string, int>> GetWordAppearanceWithFilenames(Guid corpusId, string word);
        
        Task<CollocationsInfo> GetCollocationsInfoElement(Guid corpusId, string word);
        Task<List<TokenDto>> GetCollocations(Guid corpusId, string word, int direction);
        Task<List<CollocationsMetaData>> GetCollocationsByParagraph(Guid corpusId, string word, int direction);
        Task<List<CollocationsMetaData>> GetCollocationsBySentence(Guid corpusId, string word, int direction);
    }
}