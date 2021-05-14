using System;
using Domain.Models;

namespace Application.Interfaces
{
    public interface ICorpusesRepository
    {
        void CreateCorpus(Corpus corpus);        

        Chunk GetChunkByChunkListId(Guid chunkListId, int chunkId);
        Sentence GetSentenceByChunkListAndChunkIds(Guid chunkListId, int chunkId, int sentenceId);

        bool SaveChanges();
    }
}