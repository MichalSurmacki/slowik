using System.Xml.Serialization;
using Application.Interfaces;
using Application.Dtos.Temporary;
using System.IO;
using System.Collections.Generic;
using System;
using Application.Dtos;
using Domain.Models;

namespace Application.Repositories
{
    public class CorpusesRepository : ICorpusesRepository
    {
        private readonly ISlowikContext _context;

        public CorpusesRepository(ISlowikContext context)
        {
            _context = context; 
        }

        public void CreateChunk(Chunk chunk)
        {
            if(chunk == null) throw new ArgumentNullException(nameof(chunk));
            _context.Chunks.Add(chunk);
        }

        public void CreateChunkList(ChunkList chunkList)
        {
            throw new NotImplementedException();
        }

        public void CreateCorpus(Corpus corpus)
        {
            if(corpus == null) throw new ArgumentNullException(nameof(corpus));
            _context.Corpuses.Add(corpus);
        }

        public void CreateSentence(Sentence sentence)
        {
            throw new NotImplementedException();
        }

        public Chunk GetChunkByCorpusId(Guid corpusId, int chunkId)
        {
            throw new NotImplementedException();
        }

        public Sentence GetSentenceByCorpusAndChunkIds(Guid corpusId, int chunkId, int sentenceId)
        {
            throw new NotImplementedException();
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}