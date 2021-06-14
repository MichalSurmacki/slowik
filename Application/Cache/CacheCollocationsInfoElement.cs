using System;
using System.Collections.Generic;
using Application.Dtos;
using Application.Dtos.Collocations;
using Application.Dtos.Temporary;

namespace Application.Cache
{
    public class CacheCollocationsInfoElement 
    {
        // public readonly int Direction;
        // private List<TokenDto> _collocations = new List<TokenDto>();
        // private List<CollocationsInfo> _collocationsByParagraph = new List<CollocationsInfo>();
        // private List<CollocationsInfo> _collocationsBySentence = new List<CollocationsInfo>();
        // public CacheCollocationsInfoElement(Guid corpusId, string word, int direction) : base(corpusId, word)
        // {
        //     Direction = direction;
        // }

        // public List<TokenDto> GetCollocations() => _collocations;
        // public List<CollocationsInfo> GetCollocationsBySentence() => _collocationsBySentence;
        // public List<CollocationsInfo> GetCollocationsByParagraph() => _collocationsByParagraph;

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