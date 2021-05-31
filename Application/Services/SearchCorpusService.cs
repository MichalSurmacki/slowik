using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Dtos;
using Application.Dtos.Collocations;
using Application.Dtos.Temporary;
using Application.Dtos.Words;
using Application.Interfaces;
using AutoMapper;
using Domain.Models;

namespace Application.Services
{
    public class SearchCorpusService : ISearchCorpusService
    {
        private readonly ICorpusesRepository _corpusesRepository;
        private readonly IMapper _mapper;
        public SearchCorpusService(ICorpusesRepository corpusesRepository, IMapper mapper)
        {
            _corpusesRepository = corpusesRepository;
            _mapper = mapper;
        }

        public async Task<CollocationsInfo> GetAllCollocationsAsync(Guid corpusId, string word, int direction)
        {
            return await _Look<CollocationsInfo>(corpusId, word, direction, Scope.None,
            (ChunkListMetaData chlMd, ChunkDto chDto, ref CollocationsInfo cInfo) =>
            {
                if (cInfo == null)
                        cInfo = new CollocationsInfo(corpusId, word, direction);
                 
                foreach (var sentence in chDto.Sentences)
                {
                    List<TokenDto> tokens = sentence.Tokens.ToList();
                    if (direction < 0)
                        tokens.Reverse();
                    do
                    {
                        tokens = tokens.SkipWhile(t => !t.Orth.ToLower().Equals(word.ToLower())).Skip(Math.Abs(direction)).ToList();
                        var token = tokens.FirstOrDefault();
                        if (token != null)
                        {
                            cInfo.WordCountInCorpus++;
                            if(!cInfo.Collocations.Contains(token))
                                cInfo.Collocations.Add(token);
                            if(cInfo.FilenameWithWordCountDict.ContainsKey(chlMd.OriginFileName))
                                cInfo.FilenameWithWordCountDict[chlMd.OriginFileName]++;
                            else
                                cInfo.FilenameWithWordCountDict.Add(chlMd.OriginFileName, 1);
                        }
                        tokens = tokens.Skip(1).ToList();
                    } while (tokens.Any());
                }
                return cInfo;
            });
        }

        public async Task<WordInfo> GetApperancesWithFilenamesAsync(Guid corpusId, string word)
        {
            return await _Look<WordInfo>(corpusId, word, 0, Scope.None,
            (ChunkListMetaData chlMd, ChunkDto chDto, ref WordInfo wInfo) =>
            {
                if (wInfo == null)
                        wInfo = new WordInfo(corpusId, word);
                foreach (var sentence in chDto.Sentences)
                {
                    int innerCount = sentence.Tokens.Where(t => t.Orth.ToLower().Equals(word.ToLower())).Count();
                    if(innerCount > 0)
                    {
                        wInfo.WordCountInCorpus += innerCount;
                        if(wInfo.FilenameWithWordCountDict.ContainsKey(chlMd.OriginFileName))
                            wInfo.FilenameWithWordCountDict[chlMd.OriginFileName] += innerCount;
                        else
                            wInfo.FilenameWithWordCountDict.Add(chlMd.OriginFileName, innerCount);
                    }
                }
                return wInfo;
            });
        }

        public async Task<CollocationsInfo> GetCollocationsByParagraphAsync(Guid corpusId, string word, int direction)
        {
            return await _Look<CollocationsInfo>(corpusId, word, direction, Scope.Sentence,
            (ChunkListMetaData chlMd, ChunkDto chDto, ref CollocationsInfo cInfo) =>
            {
                if (cInfo == null)
                    cInfo = new CollocationsInfo(corpusId, word, direction);

                var collocationsByParagraphMetaData = new CollocationsMetaData(chlMd.OriginFileName, chDto.XmlChunkId);
                var tokens = chDto.Sentences.SelectMany(s => s.Tokens).ToList();
                if (direction < 0)
                        tokens.Reverse();
                    do
                    {
                        tokens = tokens.SkipWhile(t => !t.Orth.ToLower().Equals(word.ToLower())).Skip(Math.Abs(direction)).ToList();
                        var token = tokens.FirstOrDefault();
                        if (token != null)
                        {
                            cInfo.WordCountInCorpus++;
                            if(cInfo.FilenameWithWordCountDict.ContainsKey(chlMd.OriginFileName))
                                cInfo.FilenameWithWordCountDict[chlMd.OriginFileName]++;
                            else
                                cInfo.FilenameWithWordCountDict.Add(chlMd.OriginFileName, 1);
                            if(!cInfo.Collocations.Contains(token))
                                cInfo.Collocations.Add(token);
                            if(!collocationsByParagraphMetaData.Collocations.Contains(token))
                                collocationsByParagraphMetaData.Collocations.Add(token);
                        }
                        tokens = tokens.Skip(1).ToList();
                    } while (tokens.Any());
                if(collocationsByParagraphMetaData.Collocations.Any())
                    cInfo.CollocationsByParagraph.Add(collocationsByParagraphMetaData);
                return cInfo;
            });
        }

        public async Task<CollocationsInfo> GetCollocationsBySentenceAsync(Guid corpusId, string word, int direction)
        {
            return await _Look<CollocationsInfo>(corpusId, word, direction, Scope.Sentence,
            (ChunkListMetaData chlMd, ChunkDto chDto, ref CollocationsInfo cInfo) =>
            {
                if (cInfo == null)
                        cInfo = new CollocationsInfo(corpusId, word, direction);
                foreach (var sentence in chDto.Sentences)
                {
                    var collocationsBySentenceMetaData = new CollocationsMetaData(chlMd.OriginFileName, chDto.XmlChunkId, sentence.XmlSentenceId);
                    List<TokenDto> tokens = sentence.Tokens.ToList();
                    if (direction < 0)
                        tokens.Reverse();
                    do
                    {
                        tokens = tokens.SkipWhile(t => !t.Orth.ToLower().Equals(word.ToLower())).Skip(Math.Abs(direction)).ToList();
                        var token = tokens.FirstOrDefault();
                        if (token != null)
                        {
                            cInfo.WordCountInCorpus++;
                            if(cInfo.FilenameWithWordCountDict.ContainsKey(chlMd.OriginFileName))
                                cInfo.FilenameWithWordCountDict[chlMd.OriginFileName]++;
                            else
                                cInfo.FilenameWithWordCountDict.Add(chlMd.OriginFileName, 1);
                            if(!cInfo.Collocations.Contains(token))
                                cInfo.Collocations.Add(token);
                            if(!collocationsBySentenceMetaData.Collocations.Contains(token))
                                collocationsBySentenceMetaData.Collocations.Add(token);
                        }
                        tokens = tokens.Skip(1).ToList();
                    } while (tokens.Any());
                    if(collocationsBySentenceMetaData.Collocations.Any())
                    cInfo.CollocationsBySentence.Add(collocationsBySentenceMetaData);
                }
                return cInfo;
            });
        }

        private delegate T _SearchDelegate<T>(ChunkListMetaData chl, ChunkDto ch, ref T input);
        private Task<T> _Look<T>(Guid corpusId, string word, int distance, Scope scope, _SearchDelegate<T> d)
        {
            var corpusChunkListMetaDatas = _corpusesRepository.GetChunkListMetaDatasByCorpusId(corpusId);
            T element = default(T);
            foreach (var c in corpusChunkListMetaDatas)
            {
                List<ChunkDto> chunksDtos;
                var chunkListMetaData = _mapper.Map<ChunkListMetaDataDto>(c);
                if (!chunkListMetaData.WordsLookupDictionary.ContainsKey(word))
                    continue;
                List<int> idsOfChunksWithWord = chunkListMetaData.WordsLookupDictionary[word];
                var chunks = _corpusesRepository.GetChunksByChunkListIdAndXmlChunkId(c.ChunkList.Id, idsOfChunksWithWord);
                chunksDtos = _mapper.Map<List<ChunkDto>>(chunks);
                foreach (var chunkDtoWithWord in chunksDtos)
                {
                    d(c, chunkDtoWithWord, ref element);
                }
            }
            return Task.FromResult(element);
        }
    }
}