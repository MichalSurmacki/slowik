using System;
using System.Collections.Generic;
using Application.Dtos;
using Application.Dtos.Temporary;

namespace Application.Cache
{
    public class CacheCollocationsInfoElement : CacheWordInfoElement
    {
        public readonly int Direction;
        public readonly Scope Scope;
        private List<TokenDto> _collocations = new List<TokenDto>();
        private Dictionary<string, List<TokenDto>> _collocationsByParagraph = new Dictionary<string, List<TokenDto>>();
        private Dictionary<string, List<TokenDto>> _collocationsBySentence = new Dictionary<string, List<TokenDto>>();
        public CacheCollocationsInfoElement(Guid corpusId, string word, int direction, Scope scope) : base(corpusId, word)
        {
            Direction = direction;
            Scope = scope;
        }

        public List<TokenDto> GetCollocations() => _collocations;
        public Dictionary<string, List<TokenDto>> GetCollocationsBySentence() => _collocationsBySentence;
        public Dictionary<string, List<TokenDto>> GetCollocationsByParagraph() => _collocationsByParagraph;

        public void AddCollocationBySentence(string sentenceId, TokenDto token)
        {
            if (_collocationsBySentence.ContainsKey(sentenceId))
            {
                _collocationsBySentence[sentenceId].Add(token);
                return;
            }
            _collocationsBySentence.Add(sentenceId, new List<TokenDto>() { token });
        }

        public void AddCollocationByParagraph(string paragraphId, TokenDto token)
        {
            if (_collocationsByParagraph.ContainsKey(paragraphId))
            {
                _collocationsByParagraph[paragraphId].Add(token);
                return;
            }
            _collocationsByParagraph.Add(paragraphId, new List<TokenDto>() { token });
        }

        public void AddCollocation(TokenDto token)
        {
            if(!_collocations.Contains(token))
                _collocations.Add(token);
        }
    }
}