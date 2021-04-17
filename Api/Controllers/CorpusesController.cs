using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Repositories;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
            _corpusesService.ParseCCLFileToObject("D:\\Desktop\\slowik-demo-data\\demo-ccl.xml");
            return Ok();
        }


        //Tutaj zwracam ID powstałego korpusu 
        [HttpPost]
        public async Task<IActionResult> CreateCorpus()
        {
            List<string> list;
            try
            {
                var file = Request.Form.Files.FirstOrDefault();
                list = await _corpusesService.ParseZIPToCCL(file.OpenReadStream());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }

            return Ok(JsonConvert.SerializeObject(list));
        }
    }
}