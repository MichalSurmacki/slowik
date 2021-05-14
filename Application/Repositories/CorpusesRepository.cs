using System.Xml.Serialization;
using Application.Interfaces;
using Application.Dtos.Temporary;
using System.IO;
using System.Collections.Generic;
using System;
using Application.Dtos;
using Domain.Models;
using System.Linq;

namespace Application.Repositories
{
    public class CorpusesRepository : ICorpusesRepository
    {
        private readonly ISlowikContext _context;

        public CorpusesRepository(ISlowikContext context)
        {
            _context = context; 
        }

        public void CreateCorpus(Corpus corpus)
        {
            if(corpus == null) throw new ArgumentNullException(nameof(corpus));
            _context.Corpuses.Add(corpus);
        }

        public Chunk GetChunkByChunkListId(Guid chunkListId, int chunkId)
        {
            return _context.Chunks.Where(c => c.Chunklist.Id.Equals(chunkListId) && 
                                         c.XmlChunkId.Equals(chunkId))
                                         .FirstOrDefault();
        }

        public Sentence GetSentenceByChunkListAndChunkIds(Guid chunkListId, int chunkId, int sentenceId)
        {
            return _context.Sentences.Where(s => s.Chunk.Id.Equals(chunkId) && 
                                            s.Chunk.Chunklist.Id.Equals(chunkListId) && 
                                            s.XmlSentenceId.Equals(sentenceId))
                                            .FirstOrDefault();
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}