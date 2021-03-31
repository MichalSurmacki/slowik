using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Application.Dtos.Temporary
{
    public class Lexem : IXmlSerializable
    {
        public int Id { get; set; }

        public string Base { get; set; }

        public CTag CTag { get; set; }

        public int Disamb { get; set; }

        public Lexem()
        {
            CTag = new CTag();
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            int disamb;
            if (Int32.TryParse(reader.GetAttribute("disamb"), out disamb)) Disamb = disamb;

            //base tag
            if (reader.Read() && reader.IsStartElement())
            {
                Base = reader.ReadElementContentAsString();
            }

            //ctag tag
            if (reader.IsStartElement())
            {
                CTag.TagNKJP = reader.ReadElementContentAsString();
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