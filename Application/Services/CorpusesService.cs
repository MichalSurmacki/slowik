using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Application.Dtos;
using Application.Dtos.Temporary;
using Application.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Application.Services
{
    //this class realizes operations - reading, searching etc. on corpuses
    public class CorpusesService : ICorpusesService
    {
        public CorpusMetaDataDto CorpusMetaData { get; set; }

        private static string baseClarinApiUri = "http://ws.clarin-pl.eu/nlprest2/base";

        private readonly IHttpClientFactory _clientFactory;

        private readonly ICorpusesRepository _corpusesRepository;

        public CorpusesService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            // _corpusesRepository = corpusesRepository;
        }

        public ChunkListDto ParseCCLFileToObject(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ChunkListDto));
            ChunkListDto result;
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                result = (ChunkListDto)serializer.Deserialize(fileStream);
            }
            return result;
        }

        public async Task<List<string>> ParseZIPToCCL(Stream stream)
        {
            var list = new List<string>();

            var archive = new ZipArchive(stream, ZipArchiveMode.Read);
            foreach (var e in archive.Entries)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    e.Open().CopyTo(ms);

                    var fileId = await UploadFileToClarinApiPostAsync(ms.ToArray());

                    var taskId = await UseClarinApiTagerPostAsync(fileId);

                    string completedTaskFileId;
                    do
                    {
                        var responseJson = await ClarinApiTaskStatusGetAsync(taskId);
                        JObject taskStatusJson = JObject.Parse(responseJson);
                        var status = taskStatusJson["status"].ToString();

                        if (status == "DONE")
                        {
                            completedTaskFileId = taskStatusJson["value"][0]["fileID"].ToString();
                            break;
                        }
                        else if (status == "ERROR")
                        {
                            var errorMessage = taskStatusJson["value"].ToString();
                            throw new Exception(errorMessage);
                        }
                        else if (status == "QUEUING" || status == "PROCESSING")
                        {
                            var message = taskStatusJson["value"].ToString();
                            Debug.WriteLine(message);
                        }
                        else
                        {
                            throw new Exception("Unknown task status form ClarinApi");
                        }
                        //czekamy sekundę może się coś zmieni
                        await Task.Delay(1000);
                    } while (true);
                    var ccl = await ClarinApiDownloadCompletedTaskGetAsync(completedTaskFileId);
                    list.Add(ccl);
                }
            }

            return list;
        }

        // zwraca id wczytanego na serwer dokumentu w formacie: "users/defalut/{guid}"
        public async Task<string> UploadFileToClarinApiPostAsync(Byte[] binaryFile)
        {
            string uri = baseClarinApiUri + "/upload";

            ByteArrayContent byteContent = new ByteArrayContent(binaryFile);
            byteContent.Headers.Add("Content-Type", "binary/octet-stream");

            var client = _clientFactory.CreateClient();

            var response = await client.PostAsync(uri, byteContent);
            var contents = await response.Content.ReadAsStringAsync();

            Debug.WriteLine("***********************************************************************");
            Debug.WriteLine(contents);
            Debug.WriteLine("***********************************************************************");
            return contents;
        }

        // zwraca id operacji przetwarzanej na serwerze w formacie "{guid}"
        public async Task<string> UseClarinApiTagerPostAsync(string uploadedFileId)
        {
            string uri = baseClarinApiUri + "/startTask";

            string json = $@"{{      
                            ""lpmn"":""any2txt|wcrft2({{\""guesser\"":false, \""morfeusz2\"":true}})"",
                            ""file"": ""{uploadedFileId}"",
                            ""user"": ""slowik-test"" 
                            }}";

            StringContent jsonContent = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.PostAsync(uri, jsonContent);
            var contents = await response.Content.ReadAsStringAsync();


            Debug.WriteLine("***********************************************************************");
            Debug.WriteLine(contents);
            Debug.WriteLine("***********************************************************************");

            return contents;
        }


        // fileId w formacie "/requests/wcrft2/{guid}", status w formacie {DONE lub ERROR (value zawiera opis błędu) lub QUEUING(?) lub 
        //PROCESSING(value posiada wartość reprezentującą stopień wykonania zadania)}
        public async Task<string> ClarinApiTaskStatusGetAsync(string taskId)
        {
            string uri = baseClarinApiUri + $"/getStatus/{taskId}";

            var client = _clientFactory.CreateClient();

            var response = await client.GetAsync(uri);
            var contents = await response.Content.ReadAsStringAsync();

            Debug.WriteLine("***********************************************************************");
            Debug.WriteLine(contents);
            Debug.WriteLine("***********************************************************************");

            return contents;
        }

        // zwraca CCL
        public async Task<string> ClarinApiDownloadCompletedTaskGetAsync(string fileId)
        {
            string uri = baseClarinApiUri + $"/download{fileId}";

            var client = _clientFactory.CreateClient();

            var response = await client.GetAsync(uri);
            var contents = await response.Content.ReadAsStringAsync();

            return contents;
        }

    }
}