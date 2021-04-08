using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Application.Dtos.Temporary
{
    [XmlRoot("sentence")]
    public class Sentence : IXmlSerializable
    {
        public int Id { get; set; }

        public List<Token> Tokens { get; set; }

        public Guid CorpusId { get; private set; }

        public int ChunkId { get; set; }

        private CorpusMetaData _corpusMetaData;

        public Sentence()
        {
            Tokens = new List<Token>();
        }

        public Sentence(ref CorpusMetaData corpusMetaData)
        {
            Tokens = new List<Token>();
            _corpusMetaData = corpusMetaData;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            
            reader.MoveToContent();
            var _xml_id = reader.GetAttribute("id");
            _xml_id = _xml_id.Trim('s');
            int _id;
            if (Int32.TryParse(_xml_id, out _id)) Id = _id;

            if(_corpusMetaData != null) _corpusMetaData.NumberOfSentences += 1;
            
            bool ns = false;

            //tok/ns tags
            while (reader.Read() && reader.IsStartElement())
            {
                switch (reader.Name)
                {
                    case "tok":
                        Token tok;
                        tok = _corpusMetaData != null ? new Token(ref _corpusMetaData, ns) : new Token();
                        tok.ReadXml(reader.ReadSubtree());
                        Tokens.Add(tok);
                        ns = false;
                        break;

                    case "ns":
                        //nastepny tok - brak spacji przed 
                        ns = true;
                        break;
                }
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