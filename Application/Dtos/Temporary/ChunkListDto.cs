using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Application.Dtos.Temporary
{
    [XmlRoot("chunkList")]
    public class ChunkListDto : IXmlSerializable
    {
        public List<ChunkDto> Chunks { get; set; }
        public ChunkListMetaDataDto _chunkListMetaData;

        public ChunkListDto()
        {
            _chunkListMetaData = new ChunkListMetaDataDto();
            Chunks = new List<ChunkDto>();
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            while (reader.Read() && reader.IsStartElement())
            {
                ChunkDto chnk = new ChunkDto(ref _chunkListMetaData);
                chnk.ReadXml(reader.ReadSubtree());
                Chunks.Add(chnk);
            }

            if (reader.NodeType == XmlNodeType.EndElement || reader.NodeType == XmlNodeType.Whitespace)
            {
                _chunkListMetaData.JsonDictionaryLookUp = JsonConvert.SerializeObject(_chunkListMetaData.WordsLookupDictionary);
                reader.Skip();
            }
        }

        //TODO: When serialization to xml is crucial this method needs to be implemented
        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}