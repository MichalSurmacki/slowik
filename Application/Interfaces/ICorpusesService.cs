using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Application.Dtos.Temporary;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface ICorpusesService
    {   
        //funkcja korzystająca z api clarin i zwracająca listę otrzymanych otagowanych XML'i
        Task<string> ParseToCCL_Async(ZipArchiveEntry zipArchiveEntry);
        Task<CorpusDto> CreateFromZIP_Async(IFormFile zipFile);
        ChunkListDto ParseCCLStringToChunkListDto(string ccl);
    }
}