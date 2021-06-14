using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Application.Cache;
using Application.Dtos;
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
        private IEmailService _emailService;

        public CorpusesController(ICorpusesService corpusesService, IEmailService emailService)
        {
            _corpusesService = corpusesService;
            _emailService = emailService;
        }

        /// <summary>
        /// Retrieves a zip file, creates corpus and returns guid key
        /// </summary>
        /// <param name="zipFile" example="example.zip">Zip file containing corpus files</param>
        /// <param name="email" example="exmlp@exmlp.exmlp">Email address for sending corpus guid</param>
        /// <response code="200">Corpus Created</response>
        /// <response code="400">Invalid file</response>
        /// <response code="500">Oops! Can't recive this corpus right now</response>
        [HttpPost]
        public async Task<IActionResult> CreateCorpus([Required] IFormFile zipFile, string email)
        {
            if (zipFile == null)
                return BadRequest();

            var corpus = await _corpusesService.CreateFromZIP_Async(zipFile);
            
            if(!String.IsNullOrEmpty(email))
                _emailService.SendCorpusGuidViaEmail(email, corpus.Id);

            if (corpus != null)
                return Ok(corpus.Id);
            else
                return BadRequest();
        }

        /// <summary>
        /// Gets colocations on the left or right by word.
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
        /// <response code="200">List of word collocations</response>
        /// <response code="400">Invalid: word or corpus or direction</response>
        /// <response code="500">Internal Server Error</response>
        [HttpGet("{corpusId:Guid}/collocations")]
        public async Task<IActionResult> GetCollocations(Guid corpusId, [FromQuery][Required] string word, [FromQuery][Required] int direction, Scope scope = Scope.None)
        {
            if (word == null || direction == 0 || corpusId == null)
                return BadRequest();

            switch(scope)
            {                    
                case Scope.Sentence:
                    var ddd = await _corpusesService.GetCollocationsBySentence_Async(corpusId, word, direction); 
                    return Ok(ddd);
                case Scope.Paragraph:
                    var dd = await _corpusesService.GetCollocationsByParagraph_Async(corpusId, word, direction);
                    return Ok(dd);
            }
            var allCollocations = await _corpusesService.GetCollocations_Async(corpusId, word, direction);
            return Ok(allCollocations);
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
        public async Task<IActionResult> GetWordAppearance(Guid corpusId, [FromQuery][Required] string word, bool groupByFiles = false)
        {
            if (word == null || corpusId == null)
                return BadRequest();

            if(groupByFiles)
            {
                return Ok(await _corpusesService.GetWordAppearanceWithFileNames_Async(corpusId, word));
            }    

            var apperances = await _corpusesService.GetWordAppearance_Async(corpusId, word);
            return Ok(new Dictionary<string, int>(){
                {"total", apperances}
            });
        }
    }
}