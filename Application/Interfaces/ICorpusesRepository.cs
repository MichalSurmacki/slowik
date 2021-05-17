using System;
using System.Collections.Generic;
using Domain.Models;

namespace Application.Interfaces
{
    public interface ICorpusesRepository
    {
        void CreateCorpus(Corpus corpus);        

        Chunk GetChunkByChunkListId(Guid chunkListId, int chunkId);
        Sentence GetSentenceByChunkListAndChunkIds(Guid chunkListId, int chunkId, int sentenceId);
        List<ChunkListMetaData> GetChunkListMetaDatasByCorpusId(Guid corpusId);
        List<Chunk> GetChunksByChunkListIdAndXmlChunkId(Guid chunkListId, List<int> xmlChunksIds);

        bool SaveChanges();
    }
}