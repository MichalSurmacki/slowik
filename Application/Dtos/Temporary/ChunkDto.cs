using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Application.Dtos.Temporary
{
    [XmlRoot("chunk")]
    public class ChunkDto : IXmlSerializable
    {
        public int XmlChunkId { get; set; }
        public List<SentenceDto> Sentences { get; set; }

        private ChunkListMetaDataDto _chunkListMetaData;

        public ChunkDto()
        {
            Sentences = new List<SentenceDto>();
        }

        public ChunkDto(ref ChunkListMetaDataDto chunkListMetaData)
        {
            _chunkListMetaData = chunkListMetaData;
            Sentences = new List<SentenceDto>();
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            var _xml_id = reader.GetAttribute("id");
            _xml_id = _xml_id.Trim('c', 'h');
            int _id;
            if (Int32.TryParse(_xml_id, out _id)) XmlChunkId = _id;

            if(_chunkListMetaData != null) _chunkListMetaData.NumberOfChunks += 1;

            while (reader.Read() && reader.IsStartElement())
            {
                SentenceDto stc = _chunkListMetaData != null ? new SentenceDto(ref _chunkListMetaData) : new SentenceDto();
                stc.ReadXml(reader.ReadSubtree());
                Sentences.Add(stc);
            }

            if (reader.NodeType == XmlNodeType.EndElement || reader.NodeType == XmlNodeType.Whitespace)
            {
                reader.Skip();
            }
        }

        //TODO: When serialization to xml is crucial this method needs to be implemented
        public void WriteXml(XmlWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }
}