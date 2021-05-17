using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Application.Dtos;
using Application.Dtos.Temporary;
using AutoMapper;
using Domain.Models;
using Newtonsoft.Json;

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

            CreateMap<ChunkListMetaDataDto, ChunkListMetaData>();

            CreateMap<ChunkListMetaData, ChunkListMetaDataDto>()
                .ForMember(dest => dest.WordsLookupDictionary,
                    opt => opt.MapFrom(src => JsonConvert.DeserializeObject<Dictionary<string, List<int>>>(src.JsonDictionaryLookUp))
                );

            CreateMap<Chunk, ChunkDto>();

            CreateMap<Sentence, SentenceDto>()
                .AfterMap((s, sdto) => 
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(SentenceDto));
                        using (StringReader reader = new StringReader(sdto.Xml))
                        {
                            sdto.ReadXml(XmlReader.Create(reader));
                        }
                    }
                );
        }
    }
}