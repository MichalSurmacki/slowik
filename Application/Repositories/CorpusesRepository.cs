using System.Xml.Serialization;
using Application.Interfaces;
using Application.Dtos.Temporary;
using System.IO;
using System.Collections.Generic;
using System;
using Application.Dtos;

namespace Application.Repositories
{
    public class CorpusesRepository : ICorpusesRepository
    {
        private readonly ICorpusesService _corpusesService;

        public CorpusesRepository(ICorpusesService corpusesService)
        {
            _corpusesService = corpusesService; 
        }

        public ChunkDto GetChunkByCorpusId(Guid corpusId, int chunkId)
        {
            throw new NotImplementedException();
        }

        public SentenceDto GetSentenceByCorpusAndChunkIds(Guid corpusId, int chunkId, int sentenceId)
        {
            throw new NotImplementedException();
        }

        public void CreateCorpusMetaData(ChunkListDto corpus)
        {
            throw new NotImplementedException();
        }
    }
}