using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Application.Dtos.Temporary
{
    [XmlRoot("tok")]
    public class TokenDto : IXmlSerializable
    {

        public string Orth { get; set; }

        public List<LexemDto> Lexems { get; set; }

        public bool NoSpaceBefore { get; set; }

        private ChunkListMetaDataDto _chunkListMetaData;

        public TokenDto()
        {
            NoSpaceBefore = false;
            _chunkListMetaData = null;
            Lexems = new List<LexemDto>();
        }

        public TokenDto(ref ChunkListMetaDataDto chunkListMetaData, bool noSpaceBefore = false)
        {
            NoSpaceBefore = noSpaceBefore;
            _chunkListMetaData = chunkListMetaData;
            Lexems = new List<LexemDto>();
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
            if (_chunkListMetaData != null)
            {
                _chunkListMetaData.NumberOfTokens += 1;
                //if word isn't in lookUpDictionary
                if (_chunkListMetaData.WordsLookupDictionary.ContainsKey(Orth))
                {
                    _chunkListMetaData.WordsLookupDictionary[Orth].Add(_chunkListMetaData.NumberOfChunks);
                }
                else
                {
                    var list = new List<int>();
                    list.Add(_chunkListMetaData.NumberOfChunks);
                    _chunkListMetaData.WordsLookupDictionary.Add(Orth, list);
                }
            }

            //lex tags
            while (reader.IsStartElement())
            {
                LexemDto lex = new LexemDto();
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