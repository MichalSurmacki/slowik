using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Application.Dtos.Temporary
{
    [XmlRoot("sentence")]
    public class SentenceDto : IXmlSerializable
    {
        public int XmlSentenceId { get; set; }
        public List<TokenDto> Tokens { get; set; }
        public string Xml { get; set; }

        private ChunkListMetaDataDto _chunkListMetaData;

        public SentenceDto()
        {
            Tokens = new List<TokenDto>();
        }

        public SentenceDto(ref ChunkListMetaDataDto chunkListMetaData)
        {
            Tokens = new List<TokenDto>();
            _chunkListMetaData = chunkListMetaData;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();
            Xml = reader.ReadOuterXml();

            XmlReader sentenceReader = XmlReader.Create(new StringReader(Xml));
            sentenceReader.MoveToContent();

            var _xml_id = sentenceReader.GetAttribute("id");
            _xml_id = _xml_id.Trim('s');
            int _id;
            if (Int32.TryParse(_xml_id, out _id)) XmlSentenceId = _id;

            if(_chunkListMetaData != null) _chunkListMetaData.NumberOfSentences += 1;
            
            bool ns = false;

            //tok/ns tags
            while (sentenceReader.Read() && sentenceReader.IsStartElement())
            {
                switch (sentenceReader.Name)
                {
                    case "tok":
                        TokenDto tok;
                        tok = _chunkListMetaData != null ? new TokenDto(ref _chunkListMetaData, ns) : new TokenDto();
                        tok.ReadXml(sentenceReader.ReadSubtree());
                        Tokens.Add(tok);
                        ns = false;
                        break;

                    case "ns":
                        //nastepny tok - brak spacji przed 
                        ns = true;
                        break;
                }
            }

            if (sentenceReader.NodeType == XmlNodeType.EndElement || sentenceReader.NodeType == XmlNodeType.Whitespace)
            {
                sentenceReader.Skip();
            }
        }

        //TODO: When serialization to xml is crucial this method needs to be implemented
        public void WriteXml(XmlWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }
}