using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Application.Dtos.Temporary
{
    [XmlRoot("chunk")]
    public class Chunk : IXmlSerializable
    {
        public int Id { get; set; }
        public List<Sentence> Sentences { get; set; }

        private CorpusMetaData _corpusMetaData;

        public Chunk()
        {
            Sentences = new List<Sentence>();
        }

        public Chunk(ref CorpusMetaData corpusMetaData)
        {
            _corpusMetaData = corpusMetaData;
            Sentences = new List<Sentence>();
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
            if (Int32.TryParse(_xml_id, out _id)) Id = _id;

            if(_corpusMetaData != null) _corpusMetaData.NumberOfChunks += 1;

            while (reader.Read() && reader.IsStartElement())
            {
                Sentence stc = _corpusMetaData != null ? new Sentence(ref _corpusMetaData) : new Sentence();
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