using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Application.Dtos.Temporary;

namespace Application.Interfaces
{
    public interface ICorpusesService
    {   
        //funkcja korzystająca z api clarin i zwracająca listę otrzymanych otagowanych XML'i
        Task<List<string>> ParseZIPToCCL(Stream stream);

        Task<Guid> CreateCorpusFromZIP(Stream stream);
        ChunkListDto ParseCCLStringToChunkListDto(string ccl);

        //CLARIN
        Task<string> UploadFileToClarinApiPostAsync(Byte[] binaryFile);
        Task<string> UseClarinApiTagerPostAsync(string uploadedFileId);
        Task<string> ClarinApiTaskStatusGetAsync(string taskId);
        Task<string> ClarinApiDownloadCompletedTaskGetAsync(string fileId);
    }
}