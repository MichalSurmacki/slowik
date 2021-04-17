using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Domain.Models;
using Infrastructure;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {

        private IMemoryCache _cache;
        private readonly SlowikContext _context;

        public DataController(IMemoryCache memoryCache, SlowikContext context)
        {
            _cache = memoryCache;
            _context = context;
        }

        [HttpGet]
        [HttpGet("Data")]
        [HttpGet("Data/{createdBy}")]
        public IActionResult Data()//string createdBy)//, string createdAt)
        {
            string createdAt = new string("test01");
            string createdBy= new string("test01");
            CorpusCashe cacheEntry;  // id  \\list of chunk-list

            // Look for cache key.
            if (!_cache.TryGetValue(createdBy+createdAt, out cacheEntry))
            {
                cacheEntry = new CorpusCashe(this, createdBy, createdAt); 
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(1)).SetSize(1);
                _cache.Set(cacheEntry.casheID, cacheEntry, cacheEntryOptions);
                return Ok("NO_Cache: "+ cacheEntry.casheID);
            }
            else
            {
                return Ok("Cache: "+ cacheEntry.casheID);
            }
        }


            private class CorpusCashe 
            {
                private DataController parent;
                public string casheID{get; set;}
                public CorpusMetaData corpusMetaData;
                public Corpus corpus;
                public List<Chunk> chunks = new List<Chunk>();
                public Dictionary<System.Guid, List<Sentence>>  sentences = new Dictionary<System.Guid, List<Sentence>>();      //chunk ID Sentences
                public CorpusCashe(DataController parent, string createdBy, string createdAt)//    init-in-constructor (in c++ unsafe xd)
                {
                    this.parent= parent;
                    casheID = createdBy+createdAt;
                    corpusMetaData = parent._context.CorpusesMetaDataXml.Where(o => o.CreatedAt.ToString()== createdAt && o.CreatedBy== createdBy).First();
                    corpus =corpusMetaData.Corpus;
                    var chunkList = parent._context.Chunklists.Where(o => o.Corpus.Id == corpus.Id).First();
                    chunks= parent._context.Chunks.Where(o => o.Id == chunkList.Id).ToList();
                    foreach(Chunk chunk in chunks)
                    {
                        sentences.Add(chunk.Id, parent._context.Sentences.Where(o => o.Chunk.Id == chunk.Id).ToList());
                    }
                    //example sentences.First().Value.First().Xml;
                    //example chunkList.ChunkListMetaData.NumberOfChunks;                  
                }
            }

    }
}