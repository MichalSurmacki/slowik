using System;
using Domain.Models;

namespace Application.Interfaces
{
    public interface ICorpusesRepository
    {
        bool SaveChanges();
        Chunk GetChunkByCorpusId(Guid corpusId, int chunkId);
        Sentence GetSentenceByCorpusAndChunkIds(Guid corpusId, int chunkId, int sentenceId);
        
        void CreateCorpus(Corpus corpus);
        void CreateChunkList(ChunkList chunkList);
        void CreateChunk(Chunk chunk);
        void CreateSentence(Sentence sentence);
    }
}