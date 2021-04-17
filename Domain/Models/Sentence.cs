using System;

namespace Domain.Models
{
    public class Sentence
    {
        public Guid Id { get; set; }
        public int XmlSentenceId { get; set; }
        public string Xml { get; set; }
        public Chunk Chunk { get; set; }
    }
}