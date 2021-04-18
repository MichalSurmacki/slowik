using Application.Dtos;
using Application.Dtos.Temporary;
using AutoMapper;
using Domain.Models;

namespace Application
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SentenceDto, Sentence>();

            CreateMap<ChunkDto, Chunk>();

            CreateMap<ChunkListDto, ChunkList>()
                .ForMember(dest => dest.ChunkListMetaData,
                 opt => opt.MapFrom(sourceMember => sourceMember._chunkListMetaData));

            CreateMap<CorpusDto, Corpus>();

            CreateMap<CorpusMetaDataDto, CorpusMetaData>();

            CreateMap<ChunkListMetaDataDto, ChunkListMetaData>()
                .ForMember(dest => dest.JsonDictionaryLookUp,
                 opt => opt.MapFrom(src => src.JsonRepresentation));

        }
    }
}