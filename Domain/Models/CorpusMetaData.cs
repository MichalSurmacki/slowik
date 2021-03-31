using System.Collections.Generic;

namespace Domain.Models
{
    public class CorpusMetaData
    {
        // klucz - szukane słowo, wartosc - lista numerow zdan gdzie wystepuje slowo
        public Dictionary<string, List<int>> LookupTable { get; set; }
    }

}