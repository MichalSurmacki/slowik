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
        private ICorpusesService _corpusesService;
        private MemoryCache _cache;

        public CorpusesController(ICorpusesService corpusesService, CorpusesCache corpusesCache)
        {
            _corpusesService = corpusesService;
            _cache = corpusesCache.Cache;
        }

        /// <summary>
        /// Retrieves a zip file, creates corpus and returns guid key
        /// </summary>
        /// <param name="zipFile" example="example.zip">The product id</param>
        /// <response code="200">Corpus Created</response>
        /// <response code="400">Invalid file</response>
        /// <response code="500">Oops! Can't recive this corpus right now</response>
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

                return Ok(corpus.Id);
            }
            else
                return BadRequest();
        }

        /// <summary>
        /// Gets colocations on the left or right by word
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET 
        ///     {
        ///        "corpusId:" : "AAAA-AAAA-AAAA-AAAA",
        ///        "word:" : "abecadło",
        ///        "scopeAndDirection": "2"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Word collocations</response>
        /// <response code="400">Invalid word/corpus/rangeAndDirection</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("{corpusId:Guid}/collocations")]
        public async Task<IActionResult> GetCollocations(Guid corpusId, string word, int rangeAndDirection, string scope)
        {
            if (word == null || rangeAndDirection == 0 || corpusId == null)
                return BadRequest();

            var collocations = await _corpusesService.GetCollocations_Async(corpusId, word, rangeAndDirection);
            return Ok(collocations);
        }

        /// <summary>
        /// Gets number of word apperances in specified corpus
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET 
        ///     {
        ///        "corpusId:" : "AAAA-AAAA-AAAA-AAAA",
        ///        "word:" : "abecadło"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Word apperance number in whole corpus</response>
        /// <response code="400">Invalid word/corpus</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("{corpusId:Guid}/apperances")]
        public async Task<IActionResult> GetWordAppearance(Guid corpusId, string word)
        {
            if (word == null || corpusId == null)
                return BadRequest();

            var apperances = await _corpusesService.GetWordAppearance_Async(corpusId, word);
            return Ok(apperances);
        }

        /// <summary>
        /// Gets numbers of word apperances in individual files contained in corpus
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET 
        ///     {
        ///        "corpusId:" : "AAAA-AAAA-AAAA-AAAA",
        ///        "word:" : "abecadło"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Word apperance number in whole corpus</response>
        /// <response code="400">Invalid word/corpus</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("{corpusId:Guid}/apperancesInFiles")]
        public async Task<IActionResult> GetWordAppearanceInFiles(Guid corpusId, string word)
        {
            return Ok(await _corpusesService.GetWordAppearanceWithFileNames_Async(corpusId, word));
        }
    }
}