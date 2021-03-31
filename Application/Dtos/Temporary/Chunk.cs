using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Application.Dtos.Temporary
{
    public class Chunk : IXmlSerializable
    {
        public int Id { get; set; }
        public List<Sentence> Sentences { get; set; }

        private CorpusMetaData _corpusMetaData;

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

            _corpusMetaData.NumberOfChunks += 1;

            while (reader.Read() && reader.IsStartElement())
            {
                Sentence stc = new Sentence(ref _corpusMetaData);
                stc.ReadXml(reader.ReadSubtree());
                Sentences.Add(stc);
            }

            if (reader.NodeType == XmlNodeType.EndElement)
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