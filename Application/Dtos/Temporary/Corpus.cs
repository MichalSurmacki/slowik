using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Application.Dtos.Temporary
{
    [XmlRoot("chunkList")]
    public class Corpus : IXmlSerializable
    {
        public Guid Id { get; set; }

        public List<Chunk> Chunks { get; set; }

        public CorpusMetaData _corpusMetaData;

        public Corpus()
        {
            Id = Guid.NewGuid();
            _corpusMetaData = new CorpusMetaData();
            _corpusMetaData.CorpusId = Id;

            Chunks = new List<Chunk>();
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
                Chunk chnk = new Chunk(ref _corpusMetaData);
                chnk.ReadXml(reader.ReadSubtree());
                Chunks.Add(chnk);
            }

            if (reader.NodeType == XmlNodeType.EndElement)
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