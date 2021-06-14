using Application.Interfaces;
using System.Collections.Generic;
using System;
using Domain.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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

        //dopisac do interface
        public CorpusMetaData GetCorpusMetaDataByCorpusId(Guid corpusId)
        {
            return _context.CorpusesMetaDataXml.Where(c => c.Corpus.Id.Equals(corpusId)).FirstOrDefault();
        } 

        //dopisac do interface
        public ChunkListMetaData GetChunkListMetaDataByChunkListId(Guid chunkListId)
        {
            return _context.ChunkListMetaData.Where(c => c.ChunkList.Id.Equals(chunkListId)).FirstOrDefault();
        }

        public List<ChunkListMetaData> GetChunkListMetaDatasByCorpusId(Guid corpusId)
        {
            var chunkLists = _context.Chunklists
                .Include(c => c.ChunkListMetaData)
                .Where(c => c.Corpus.Id.Equals(corpusId)).ToList();
            return chunkLists.Select(c => c.ChunkListMetaData).ToList();
        }

        public List<Chunk> GetChunksByChunkListIdAndXmlChunkId(Guid chunkListId, List<int> xmlChunksIds)
        {
            return _context.Chunks
                .Include(s => s.Sentences)
                .Where(c => c.Chunklist.Id.Equals(chunkListId) && xmlChunksIds.Contains(c.XmlChunkId)).ToList();
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}