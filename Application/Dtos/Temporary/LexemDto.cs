using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Application.Dtos.Temporary
{
    [XmlRoot("lex")]
    public class LexemDto : IXmlSerializable
    {
        public string Base { get; set; }

        public string CTag { get; set; }

        public int Disamb { get; set; }

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
                CTag = reader.ReadElementContentAsString();
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

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            
            LexemDto t = (LexemDto)obj;
            return (Base == t.Base) && (CTag == t.CTag) && (Disamb == t.Disamb);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        } 

        public override string ToString()
        {
            return base.ToString();
        }
    }
}