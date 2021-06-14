using System;
using System.Collections.Generic;

namespace Application.Cache
{
    public class CacheWordInfoElement
    {
        public Guid CorpusId { get; }
        public string Word { get; }
        public int WordCountInCorpus { get; set; } = 0;
        private Dictionary<string, int> _filenameWithWordCountDict = new Dictionary<string, int>();

        public CacheWordInfoElement(Guid corpusId, string word)
        {
            CorpusId = corpusId;
            Word = word;
        }

        public void AddApperanceInFilename(string fileName, int count = 1)
        {
            if(_filenameWithWordCountDict.ContainsKey(fileName))
            {
                _filenameWithWordCountDict[fileName] += count;
                return;
            }
            _filenameWithWordCountDict.Add(fileName, 1);
        }

        public Dictionary<string, int> GetApperanceInFilesDict()
        {
            return _filenameWithWordCountDict;
        }
    }
}