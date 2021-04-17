using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Application.Dtos.Temporary
{
    [XmlRoot("chunkList")]
    public class ChunkListDto : IXmlSerializable
    {
        public Guid Id { get; set; }

        public List<ChunkDto> Chunks { get; set; }

        public CorpusMetaDataDto _corpusMetaData;

        public ChunkListDto()
        {
            Id = Guid.NewGuid();
            _corpusMetaData = new CorpusMetaDataDto();
            _corpusMetaData.CorpusId = Id;

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
                ChunkDto chnk = new ChunkDto(ref _corpusMetaData);
                chnk.ReadXml(reader.ReadSubtree());
                Chunks.Add(chnk);
            }

            if (reader.NodeType == XmlNodeType.EndElement || reader.NodeType == XmlNodeType.Whitespace)
            {
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