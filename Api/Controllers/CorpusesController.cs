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

            var corpus = await _corpusesService.CreateFromZIPAsync(zipFile.OpenReadStream());

            if (corpus != null)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSize(1);
                _cache.Set<CorpusDto>(corpus.Id, corpus, cacheEntryOptions);

                return Ok(new Tuple<string, Guid>("CorpusGuid", corpus.Id));
            }
            else
                return BadRequest();
        }
    }
}