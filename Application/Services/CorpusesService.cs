using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Application.Dtos;
using Application.Dtos.Clarin;
using Application.Dtos.Temporary;
using Application.Interfaces;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    //this class realizes operations - reading, searching etc. on corpuses
    public class CorpusesService : ICorpusesService
    {
        public CorpusMetaDataDto CorpusMetaData { get; set; }

        private readonly IClarinService _clarinService;
        private readonly ICorpusesRepository _corpusesRepository;
        private readonly IMapper _mapper;

        public CorpusesService(IClarinService clarinService, ICorpusesRepository corpusesRepository,
                                IMapper mapper)
        {
            _clarinService = clarinService;
            _corpusesRepository = corpusesRepository;
            _mapper = mapper;
        }

        public ChunkListDto ParseCCLStringToChunkListDto(string ccl)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ChunkListDto));
            ChunkListDto result;
            using (StringReader reader = new StringReader(ccl))
            {
                result = (ChunkListDto)serializer.Deserialize(reader);
            }

            return result;
        }

        public async Task<CorpusDto> CreateFromZIP_Async(IFormFile zipFile)
        {
            CorpusDto corpusDto = new CorpusDto();
            try
            {
                var archive = new ZipArchive(zipFile.OpenReadStream(), ZipArchiveMode.Read);
                var CCLs = new List<string>();
                foreach (var e in archive.Entries)
                {
                    var ccl = await ParseToCCL_Async(e);
                    CCLs.Add(ccl);
                    var chunkListDto = ParseCCLStringToChunkListDto(ccl);
                    chunkListDto._chunkListMetaData.OriginFileName = e.Name;
                    corpusDto.ChunkLists.Add(chunkListDto);
                }

                corpusDto.CorpusMetaData = new CorpusMetaDataDto(corpusDto, zipFile.FileName, "anybody");

                // database changes
                Corpus corpus = _mapper.Map<CorpusDto, Corpus>(corpusDto);
                _corpusesRepository.CreateCorpus(corpus);
                _corpusesRepository.SaveChanges();
                corpusDto.Id = corpus.Id;

                return corpusDto;
            }
            catch (Exception ex)
            {
                if (ex is ArgumentException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException || ex is InvalidDataException)
                    return null;
                throw;
            }
        }

        public async Task<string> ParseToCCL_Async(ZipArchiveEntry entry)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                entry.Open().CopyTo(ms);

                var fileId = await _clarinService.UploadFile_ApiPostAsync(ms.ToArray());
                var taskId = await _clarinService.UseWCRFT2Tager_ApiPostAsync(fileId);

                TaskStatusDto taskStatus;
                string ccl = "";
                do
                {
                    taskStatus = await _clarinService.GetTaskStatus_ApiGetAsync(taskId);
                    if (taskStatus.Status == "ERROR")
                    {
                        ccl = $"CLARIN ERROR WHILE PARSING|{entry.Name}";
                        break;
                    }
                    else if (taskStatus.UnknowStatus)
                    {
                        ccl = $"CLARIN UNKNOW STATUS WHILE PARSING:|{entry.Name}";
                        break;
                    }
                    else if (taskStatus.Status != "DONE")
                    {
                        //czekamy pół sekundy może się coś zmieni
                        await Task.Delay(500);
                    }

                } while (taskStatus.Status != "DONE");

                if (taskStatus.Status != "ERROR")
                    ccl = await _clarinService.DownloadCompletedTask_ApiGetAsync(taskStatus.ResultFileId);

                return ccl;
            }
        }
    }
}