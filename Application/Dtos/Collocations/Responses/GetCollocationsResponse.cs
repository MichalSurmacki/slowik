using System.Collections.Generic;
using Application.Dtos.Temporary;

namespace Application.Dtos.Collocations.Responses
{
    public class GetCollocationsResponse
    { 
        public List<TokenDto> AllCollocations { get; set; }
        public List<CollocationsMetaData> Collocations { get; set; }
    }
}