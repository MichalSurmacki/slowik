using System;
using System.Threading.Tasks;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CorpusesController : ControllerBase
    {
        private ICorpusesRepository _corpusesRepository;
        private ICorpusesService _corpusesService;

        public CorpusesController(ICorpusesRepository corpusesRepository, ICorpusesService corpusesService)
        {
            _corpusesRepository = corpusesRepository;
            _corpusesService = corpusesService;
        }

        [HttpGet]
        public ActionResult<string> Home()
        {
            return Ok();
        }

        /// <summary>
        /// Creates new coprus. 
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCorpus(IFormFile zipFile)
        {
            if (zipFile == null)
                return BadRequest();

            var corpus = await _corpusesService.CreateFromZIPAsync(zipFile.OpenReadStream());

            if (corpus != null)
                return Ok(new Tuple<string, Guid>("CorpusGuid", corpus.Id));
            else
                return BadRequest();
        }
    }
}