using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Clarin;
using Application.Interfaces;
using Newtonsoft.Json.Linq;

namespace Application.Services
{
    public class ClarinService : IClarinService
    {
        private static string baseClarinApiUri = "http://ws.clarin-pl.eu/nlprest2/base";

        private readonly IHttpClientFactory _clientFactory;

        public ClarinService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<string> DownloadCompletedTask_ApiGetAsync(string fileId)
        {
            string uri = baseClarinApiUri + $"/download{fileId}";

            var client = _clientFactory.CreateClient();

            var response = await client.GetAsync(uri);
            var contents = await response.Content.ReadAsStringAsync();

            Debug.WriteLine("****************************** DownloadCompletedTask_ApiGetAsync *****************************************");

            return contents;
        }

        public async Task<TaskStatusDto> GetTaskStatus_ApiGetAsync(Guid taskGuid)
        {
            string uri = baseClarinApiUri + $"/getStatus/{taskGuid}";

            var client = _clientFactory.CreateClient();

            var response = await client.GetAsync(uri);
            var contents = await response.Content.ReadAsStringAsync();

            Debug.WriteLine("****************************** GetTaskStatus_ApiGetAsync *****************************************");
            Debug.WriteLine(contents);

            JObject jsonResponse = JObject.Parse(contents);
            TaskStatusDto taskStatus = new TaskStatusDto();
            taskStatus.Status = jsonResponse["status"].ToString();
            
            if(taskStatus.Status == "DONE")
                taskStatus.ResultFileId = jsonResponse["value"][0]["fileID"].ToString();
            else if(taskStatus.Status == "PROCESSING")
                taskStatus.ProcessingValue = jsonResponse["value"].ToString();
            else if(taskStatus.Status == "ERROR")
                taskStatus.ErrorMessage = jsonResponse["value"].ToString();
            else
                taskStatus.UnknowStatus = true;

            return taskStatus;
        }

        // zwraca id wczytanego na serwer dokumentu w formacie: "users/defalut/{guid}"
        public async Task<string> UploadFile_ApiPostAsync(byte[] binaryFile)
        {
            string uri = baseClarinApiUri + "/upload";

            ByteArrayContent byteContent = new ByteArrayContent(binaryFile);
            byteContent.Headers.Add("Content-Type", "binary/octet-stream");

            var client = _clientFactory.CreateClient();

            var response = await client.PostAsync(uri, byteContent);
            var contents = await response.Content.ReadAsStringAsync();

            Debug.WriteLine("******************************* UploadFile_ApiPostAsync ****************************************");
            Debug.WriteLine(contents);
            return contents;
        }

        public async Task<Guid> UseWCRFT2Tager_ApiPostAsync(string uploadedFileId)
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


            Debug.WriteLine("********************************* UseWCRFT2Tager_ApiPostAsync **************************************");
            Debug.WriteLine(contents);

            return Guid.Parse(contents);
        }
    }
}