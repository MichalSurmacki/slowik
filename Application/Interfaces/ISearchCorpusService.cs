using System;
using System.Threading.Tasks;
using Application.Dtos.Collocations;
using Application.Dtos.Words;

namespace Application.Interfaces
{
    /// <summary>
    /// <para/>Methods in here always returns CollocationsInfo or WordInfo in order to reduce operations of searching thru corpus.
    /// Caching of CollocationsInfo and WordInfo gives opportunity to return information faster without the need for searching thru corpus again.
    /// <para/>e.g. Searching for collocations in scope of sentence gives possibility to store: all collocations, word count in corpus, word count in specific files in corpus.
    /// </summary>
    public interface ISearchCorpusService
    {
        Task<CollocationsInfo>  GetAllCollocationsAsync(Guid corpusId, string word, int direction);
        Task<CollocationsInfo> GetCollocationsBySentenceAsync(Guid corpusId, string word, int direction);
        Task<CollocationsInfo> GetCollocationsByParagraphAsync(Guid corpusId, string word, int direction);
        Task<WordInfo> GetApperancesWithFilenamesAsync(Guid corpusId, string word);
    }
}