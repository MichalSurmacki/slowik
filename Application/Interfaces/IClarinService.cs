using System;
using System.IO.Compression;
using System.Threading.Tasks;
using Application.Dtos.Clarin;

namespace Application.Interfaces
{
    public interface IClarinService
    {
        Task<string> GetCCLStringFromZipArchiveEntry(ZipArchiveEntry entry);


        /// <summary>
        /// Makes 'post request' to http://ws.clarin-pl.eu/nlprest2/base/upload.
        /// Posts binary file (which contain text to process) with header: Content-Type: binary/octet-stream
        /// </summary>
        /// <returns>File id for usage while proccessing. Example: /users/default/{guid}</returns>
        Task<string> UploadFile_ApiPostAsync(Byte[] binaryFile);

        /// <summary>
        /// Makes 'post request' to http://ws.clarin-pl.eu/nlprest2/base/startTask. 
        /// Posts json containing info with fileId, taskToStart, userWhoStartTask
        /// <example> 
        /// <code>
        /// Json used for WCRFT2 tager:
        /// {      
        ///     "lpmn":"any2txt|wcrft2({\"guesser\":false, \"morfeusz2\":true})",
        ///     "file": "/users/default/{guid}",
        ///     "user": "UserName" 
        /// } 
        /// </code>
        /// </example>
        /// </summary>
        /// <returns>Guid of started task</returns>
        Task<Guid> UseWCRFT2Tager_ApiPostAsync(string uploadedFileId);

        // fileId w formacie "/requests/wcrft2/{guid}", status w formacie {DONE lub ERROR (value zawiera opis błędu) lub QUEUING(?) lub 
        //PROCESSING(value posiada wartość reprezentującą stopień wykonania zadania)}
        /// <summary>
        /// Makes 'get request' to http://ws.clarin-pl.eu/nlprest2/base/getStatus/{guid}. 
        /// {guid} is Guid of Task started by UseClarinWCRFT2Tager_ApiPostAsync function.
        /// </summary>
        /// <returns>Json containing info about file which is proccessed in Task and Status of Task</returns>
        Task<TaskStatusDto> GetTaskStatus_ApiGetAsync(Guid taskGuid);
        
        /// <summary>
        /// Makes 'get request' to http://ws.clarin-pl.eu/nlprest2/base/download{fileId}. 
        /// Example of {fileId}: '/requests/wcrft2/{guid}'
        /// </summary>
        /// <returns>String containing tagged text in XML (CCL) format</returns>
        Task<string> DownloadCompletedTask_ApiGetAsync(string fileId);
    }
}