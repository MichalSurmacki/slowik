using System;
using System.Collections.Generic;
using Application.Dtos.Temporary;
using Application.Dtos.Words;

namespace Application.Dtos.Collocations
{
    public class CollocationsInfo : WordInfo
    {
        public readonly int Direction;
        public List<TokenDto> Collocations { get; set; } = new List<TokenDto>();
        public List<CollocationsMetaData> CollocationsByParagraph { get; set; } = new List<CollocationsMetaData>();
        public List<CollocationsMetaData> CollocationsBySentence { get; set; } = new List<CollocationsMetaData>();
        public CollocationsInfo(Guid corpusId, string word, int direction) : base(corpusId, word)
        {
            Direction = direction;
        }

        // public List<TokenDto> GetCollocations() => _collocations;
        // public List<CollocationsData> GetCollocationsBySentence() => _collocationsBySentence;
        // public List<CollocationsData> GetCollocationsByParagraph() => _collocationsByParagraph;

        // public void AddCollocationBySentence(string sentenceId, TokenDto token)
        // {
        //     if (_collocationsBySentence.ContainsKey(sentenceId))
        //     {
        //         _collocationsBySentence[sentenceId].Add(token);
        //         return;
        //     }
        //     _collocationsBySentence.Add(sentenceId, new List<TokenDto>() { token });
        // }

        // public void AddCollocationByParagraph(string paragraphId, TokenDto token)
        // {
        //     if (_collocationsByParagraph.ContainsKey(paragraphId))
        //     {
        //         _collocationsByParagraph[paragraphId].Add(token);
        //         return;
        //     }
        //     _collocationsByParagraph.Add(paragraphId, new List<TokenDto>() { token });
        // }

        // public void AddCollocation(TokenDto token)
        // {
        //     if(!_collocations.Contains(token))
        //         _collocations.Add(token);
        // }
    }
}