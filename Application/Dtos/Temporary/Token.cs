using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Application.Dtos.Temporary
{
    [XmlRoot("tok")]
    public class Token : IXmlSerializable
    {

        public string Orth { get; set; }

        public List<Lexem> Lexems { get; set; }

        public bool NoSpaceBefore { get; set; }

        private CorpusMetaData _corpusMetaData;

        public Token()
        {
            NoSpaceBefore = false;
            _corpusMetaData = null;
            Lexems = new List<Lexem>();
        }

        public Token(ref CorpusMetaData corpusMetaData, bool noSpaceBefore = false)
        {
            NoSpaceBefore = noSpaceBefore;
            _corpusMetaData = corpusMetaData;
            Lexems = new List<Lexem>();
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            //orth tag
            if (reader.Read() && reader.IsStartElement())
            {
                Orth = reader.ReadElementContentAsString();
            }

            //if string is parsed first time
            if (_corpusMetaData != null)
            {
                _corpusMetaData.NumberOfTokens += 1;
                //if word isn't in lookUpDictionary
                if (_corpusMetaData.WordsLookupDictionary.ContainsKey(Orth))
                {
                    _corpusMetaData.WordsLookupDictionary[Orth].Add(_corpusMetaData.NumberOfChunks);
                }
                else
                {
                    var list = new List<int>();
                    list.Add(_corpusMetaData.NumberOfChunks);
                    _corpusMetaData.WordsLookupDictionary.Add(Orth, list);
                }
            }

            //lex tags
            while (reader.IsStartElement())
            {
                Lexem lex = new Lexem();
                lex.ReadXml(reader.ReadSubtree());
                Lexems.Add(lex);

                if (reader.NodeType == XmlNodeType.EndElement || reader.NodeType == XmlNodeType.Whitespace)
                {
                    reader.Skip();
                }
            }
        }

        //TODO: When serialization to xml is crucial this method needs to be implemented
        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}