using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class CorpusMetaData
    {
        public Guid Id { get; set; }
        public string OriginFileName { get; set; }
        public int NumberOfProcessedFiles { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public Corpus Corpus { get; set; }
    }

}