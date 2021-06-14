using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Threading.Tasks;
using Application.Dtos.Collocations;
using Application.Dtos.Temporary;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface ICorpusesService
    {   
        //funkcja korzystająca z api clarin i zwracająca listę otrzymanych otagowanych XML'i
        Task<CorpusDto> CreateFromZIP_Async(IFormFile zipFile);
        ChunkListDto ParseCCLStringToChunkListDto(string ccl);

        Task<List<TokenDto>> GetCollocations_Async(Guid corpusId, string word, int direction);
        Task<List<CollocationsMetaData>> GetCollocationsBySentence_Async(Guid corpusId, string word, int direction);
        Task<List<CollocationsMetaData>> GetCollocationsByParagraph_Async(Guid corpusId, string word, int direction);
        
        Task<int> GetWordAppearance_Async(Guid corpusId, string word);
        Task<Dictionary<string, int>> GetWordAppearanceWithFileNames_Async(Guid corpusId, string word);

        //
    }
}