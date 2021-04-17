using System;
using Application.Dtos.Temporary;

namespace Application.Interfaces
{
    public interface ICorpusesRepository
    {
        ChunkDto GetChunkByCorpusId(Guid corpusId, int chunkId);
        SentenceDto GetSentenceByCorpusAndChunkIds(Guid corpusId, int chunkId, int sentenceId);
        void CreateCorpusMetaData(ChunkListDto corpus);
    }
}