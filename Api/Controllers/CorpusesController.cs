using System;
using System.Threading.Tasks;
using Application.Cache;
using Application.Dtos.Temporary;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CorpusesController : ControllerBase
    {
        private ICorpusesRepository _corpusesRepository;
        private ICorpusesService _corpusesService;
        private MemoryCache _cache;

        public CorpusesController(ICorpusesRepository corpusesRepository, ICorpusesService corpusesService, CorpusesCache corpusesCache)
        {
            _corpusesRepository = corpusesRepository;
            _corpusesService = corpusesService;
            _cache = corpusesCache.Cache;
        }

        [HttpGet]
        public ActionResult<string> Home()
        {
            return Ok();
        }

        /// Summary:
        ///     Creates new coprus from zip file.
        [HttpPost]
        public async Task<IActionResult> CreateCorpus(IFormFile zipFile)
        {
            if (zipFile == null)
                return BadRequest(); 

            var corpus = await _corpusesService.CreateFromZIP_Async(zipFile);

            if (corpus != null)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1);
                _cache.Set<CorpusDto>(corpus.Id, corpus, cacheEntryOptions);

                return Ok(new Tuple<string, Guid>("CorpusGuid", corpus.Id));
            }
            else
                return BadRequest();
        }

        /// Summary:
        ///     Gets colocations on the left or right by word
        [HttpGet("{corpusId:Guid}/collocations")]
        public async Task<IActionResult> GetCollocations(Guid corpusId, string word, int scopeAndDirection)
        {
            if(word == null || scopeAndDirection == 0 || corpusId == null)
                return BadRequest();

            var collocations = await _corpusesService.GetCollocationsWithDistance(corpusId, word, scopeAndDirection);
            return Ok(collocations);
        }

        /// Summary:
        ///     Gets
        [HttpGet("{corpusId:Guid}/apperances")]
        public async Task<IActionResult> GetNumberOfAppearance(Guid corpusId, string word)
        {
            if(word == null || corpusId == null)
                return BadRequest();

            var apperances = await _corpusesService.GetNumberOfAppearance(corpusId, word);
            return Ok(apperances);
        }
    }
}