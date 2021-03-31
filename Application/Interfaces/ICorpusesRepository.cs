using System;
using Application.Dtos.Temporary;

namespace Application.Interfaces
{
    public interface ICorpusesRepository
    {
        Chunk GetChunkByCorpusId(Guid corpusId, int chunkId);
        Sentence GetSentenceByCorpusAndChunkIds(Guid corpusId, int chunkId, int sentenceId);
        void CreateCorpusMetaData(Corpus corpus);
    }
}