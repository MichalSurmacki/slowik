using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Application.Dtos.Temporary;

namespace Application.Interfaces
{
    public interface ICorpusesService
    {   
        //funkcja korzystająca z api clarin i zwracająca listę otrzymanych otagowanych XML'i
        Task<List<string>> ParseZIPToCCLAsync(ZipArchive archive);
        Task<CorpusDto> CreateFromZIPAsync(Stream stream);
        ChunkListDto ParseCCLStringToChunkListDto(string ccl);
    }
}