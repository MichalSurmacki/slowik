using System;
using System.Threading.Tasks;
using Application.Cache;
using Application.Dtos.Temporary;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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

        /// <summary>
        /// Retrieves a zip file and returns key
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Todo
        ///     {
        ///        "TODO:"
        ///     }
        ///
        /// </remarks>
        /// <param name="zipFile" example="example.zip">The product id</param>
        /// <response code="200">Zip saved</response>
        /// <response code="400">Invalid file</response>
        /// <response code="500">Oops! Can't recive this corpus right now</response>
        [HttpPost]
        [ProducesResponseType(typeof(Tuple<string, Guid>),200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateCorpus([BindRequired]IFormFile zipFile)
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


        /// <summary>
        /// Gets colocations on the left or right by word
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST 
        ///     {
        ///        "Guid:" : "AAAA-AAAA-AAAA-AAAA",
        ///        "word:" : "abecadło",
        ///        "scopeAndDirection": "2"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Word collocations</response>
        /// <response code="400">Invalid word/corpus/scopeAndDirection</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("{corpusId:Guid}/collocations")]
        public async Task<IActionResult> GetCollocations([BindRequired]Guid corpusId, [BindRequired]string word, [BindRequired]int scopeAndDirection)
        {
            if(word == null || scopeAndDirection == 0 || corpusId == null)
                return BadRequest();

            var collocations = await _corpusesService.GetCollocationsWithDistance(corpusId, word, scopeAndDirection);
            return Ok(collocations);
        }

        /// <summary>
        /// Gets number of word apperances
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST 
        ///     {
        ///        "Guid:" : "AAAA-AAAA-AAAA-AAAA",
        ///        "word:" : "abecadło"
        ///     }
        ///
        /// </remarks>
        /// <response code="200">Word apperance number</response>
        /// <response code="400">Invalid word/corpus</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("{corpusId:Guid}/apperances")]
        public async Task<IActionResult> GetNumberOfAppearance([BindRequired]Guid corpusId, [BindRequired]string word)
        {
            if(word == null || corpusId == null)
                return BadRequest();

            var apperances = await _corpusesService.GetNumberOfAppearance(corpusId, word);
            return Ok(apperances);
        }
    }
}