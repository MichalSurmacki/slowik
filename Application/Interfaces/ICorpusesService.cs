using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading.Tasks;
using Application.Dtos.Temporary;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface ICorpusesService
    {   
        //funkcja korzystająca z api clarin i zwracająca listę otrzymanych otagowanych XML'i
        Task<CorpusDto> CreateFromZIP_Async(IFormFile zipFile);
        Task<string> ParseToCCL_Async(ZipArchiveEntry zipArchiveEntry);
        ChunkListDto ParseCCLStringToChunkListDto(string ccl);

        Task<List<TokenDto>> GetCollocations_Async(Guid corpusId, string word, int distance);
        Task<List<List<TokenDto>>> GetCollocationsBySentence_Async(Guid corpusId, string word, int distance);
        Task<List<List<TokenDto>>> GetCollocationsByParagraph_Async(Guid corpusId, string word, int distance);
        
        Task<int> GetWordAppearance_Async(Guid corpusId, string word);
        Task<Dictionary<string, int>> GetWordAppearanceWithFileNames_Async(Guid corpusId, string word);

        //
    }
}