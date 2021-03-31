using System;
using System.Linq;
using Application.Interfaces;
using Application.Repositories;
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
        public ActionResult <string> Home()
        {
            _corpusesService.ParseCCLFileToObject("");
            return Ok("test");
        }

        [HttpPost]
        public IActionResult CreateCorpus()
        {
            try
            {
                var file = Request.Form.Files.FirstOrDefault();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
            return Ok();
        }
    }
}