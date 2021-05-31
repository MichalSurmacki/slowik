using System;
using System.Collections.Generic;

namespace Application.Dtos.Words
{
    public class WordInfo
    {
        public readonly Guid CorpusId;
        public readonly string Word;
        public int WordCountInCorpus { get; set; } = 0;
        public Dictionary<string, int> FilenameWithWordCountDict { get; set; } = new Dictionary<string, int>();

        public WordInfo(Guid corpusId, string word)
        {
            CorpusId = corpusId;
            Word = word;
        }

        // public void AddApperanceInFilename(string fileName, int count = 1)
        // {
        //     if(_filenameWithWordCountDict.ContainsKey(fileName))
        //     {
        //         _filenameWithWordCountDict[fileName] += count;
        //         return;
        //     }
        //     _filenameWithWordCountDict.Add(fileName, 1);
        // }

        // public Dictionary<string, int> GetApperanceInFilesDict()
        // {
        //     return _filenameWithWordCountDict;
        // }
    }
}