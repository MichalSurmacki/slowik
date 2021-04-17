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
            CreateMap<SentenceDto, Sentence>()
                .ForMember(dest => dest.XmlSentenceId,
                 opt=>opt.MapFrom(src => src.Id));
            
            CreateMap<ChunkDto, Chunk>()
                .ForMember(dest => dest.XmlChunkId,
                 opt=>opt.MapFrom(src => src.Id));

            CreateMap<ChunkListDto, ChunkList>();

            CreateMap<CorpusDto, Corpus>();

            CreateMap<CorpusMetaDataDto, CorpusMetaData>();

            CreateMap<ChunkListMetaDataDto, ChunkListMetaData>();

        }
    }
}