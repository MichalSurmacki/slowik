using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Application.Dtos.Temporary;
using Xunit;
using FluentAssertions;
using System;

namespace Application.Tests
{
    public class TemporaryDtosDeserializationTests
    {
        [Theory]
        [InlineData("TEST1", "TEST11", 1, @"<lex disamb=""1"">
                                            <base>TEST1</base>
                                            <ctag>TEST11</ctag>
                                           </lex>")]
        [InlineData("@#$%", "@#$%", 999, @"<lex disamb=""999"">
                                            <base>@#$%</base>
                                            <ctag>@#$%</ctag>
                                           </lex>")]
        [InlineData("żółć", "subst:sg:gen:f", 1, @"<lex disamb=""1"">
                                            <base>żółć</base>
                                            <ctag>subst:sg:gen:f</ctag>
                                           </lex>")]
        public void Lexem_ReadXml_ShouldDeserializeXML(string expectedBase, string expectedCtag, int expectedDisamb, string xml)
        {
            // Arange
            XmlSerializer serializer = new XmlSerializer(typeof(LexemDto));
            LexemDto result;

            // Act
            using (StringReader fileStream = new StringReader(xml))
            {
                result = (LexemDto)serializer.Deserialize(fileStream);
            }

            // Assert
            result.Disamb.Should().Be(expectedDisamb);
            result.CTag.Should().Be(expectedCtag);
            result.Base.Should().Be(expectedBase);
        }

        [Theory]
        [InlineData("TEST1", 2, @"<tok>
                            <orth>TEST1</orth>
                            <lex disamb=""1"">
                                <base>TEST11</base>
                                <ctag>TEST111</ctag>
                            </lex>
                            <lex disamb=""1"">
                                <base>TEST11</base>
                                <ctag>TEST111</ctag>
                            </lex>
                        </tok>")]
        [InlineData("Żółciasty", 1, @"<tok>
                            <orth>Żółciasty</orth>
                            <lex disamb=""1"">
                                <base>TEST11</base>
                                <ctag>TEST111</ctag>
                            </lex>
                        </tok>")]
        public void Token_ReadXml_ShouldDeserializeXMLWithoutCorpusMetaData(string expectedOrth, int expectedNumberOfLexems, string xml)
        {
            // Arange
            XmlSerializer serializer = new XmlSerializer(typeof(TokenDto));
            TokenDto result;

            // Act
            using (StringReader fileStream = new StringReader(xml))
            {
                result = (TokenDto)serializer.Deserialize(fileStream);
            }

            // Assert
            result.Orth.Should().Be(expectedOrth);
            result.NoSpaceBefore.Should().BeFalse();
            result.Lexems.Should().HaveCount(expectedNumberOfLexems);
        }

        [Theory]
        [InlineData(1, 5, @"<sentence id=""s1"">
                        <ns/>
                        <tok>
                            <orth>Woda</orth>
                            <lex disamb=""1""><base>woda</base><ctag>subst:sg:nom:f</ctag></lex>
                        </tok>
                        <tok>
                            <orth>jedną</orth>
                            <lex disamb=""1""><base>jeden</base><ctag>adj:sg:acc:f:pos</ctag></lex>
                        </tok>
                        <tok>
                            <orth>we</orth>
                            <lex disamb=""1""><base>w</base><ctag>prep:acc:wok</ctag></lex>
                        </tok>
                        <tok>
                            <orth>Wszechświecie</orth>
                            <lex disamb=""1""><base>wszechświat</base><ctag>subst:sg:loc:m3</ctag></lex>
                        </tok>
                        <ns/>
                        <tok>
                            <orth>.</orth>
                            <lex disamb=""1""><base>.</base><ctag>interp</ctag></lex>
                        </tok>
                        </sentence>")]
        [InlineData(70, 2, @"<sentence id=""s70"">
                        <ns/>
                        <tok>
                            <orth>Woda</orth>
                            <lex disamb=""1""><base>woda</base><ctag>subst:sg:nom:f</ctag></lex>
                        </tok>
                        <ns/>
                        <tok>
                            <orth>.</orth>
                            <lex disamb=""1""><base>.</base><ctag>interp</ctag></lex>
                        </tok>
                        </sentence>")]
        public void Sentence_ReadXml_ShouldDeserializeXMLWithoutCorpusMetaData(int expectedSentenceId, int expectedNumberOfTokens, string xml)
        {
            // Arange
            XmlSerializer serializer = new XmlSerializer(typeof(SentenceDto));
            SentenceDto result;

            // Act
            using (StringReader fileStream = new StringReader(xml))
            {
                result = (SentenceDto)serializer.Deserialize(fileStream);
            }

            // Assert
            result.XmlSentenceId.Should().Be(expectedSentenceId);
            result.Tokens.Should().HaveCount(expectedNumberOfTokens);
        }

        [Theory]
        [InlineData(1, 1, @"<chunk id=""ch1"" type=""p"">
                        <sentence id=""s1"">
                        <ns/>
                        <tok>
                            <orth>Woda</orth>
                            <lex disamb=""1""><base>woda</base><ctag>subst:sg:nom:f</ctag></lex>
                        </tok>
                        <tok>
                            <orth>jest</orth>
                            <lex disamb=""1""><base>być</base><ctag>fin:sg:ter:imperf</ctag></lex>
                        </tok>
                        </sentence>
                        </chunk>")]
        [InlineData(22, 2, @"<chunk id=""ch22"" type=""p"">
                        <sentence id=""s1"">
                        <ns/>
                        <tok>
                            <orth>Woda</orth>
                            <lex disamb=""1""><base>woda</base><ctag>subst:sg:nom:f</ctag></lex>
                        </tok>
                        </sentence>
                        <sentence id=""s2"">
                        <ns/>
                        <tok>
                            <orth>Woda</orth>
                            <lex disamb=""1""><base>woda</base><ctag>subst:sg:nom:f</ctag></lex>
                        </tok>
                        </sentence>
                        </chunk>")]
        public void Chunk_ReadXml_ShouldDeserializeXMLWithoutCorpusMetaData(int expectedChunkId, int expectedNumberOfSentences, string xml)
        {
            // Arange
            XmlSerializer serializer = new XmlSerializer(typeof(ChunkDto));
            ChunkDto result;

            // Act
            using (StringReader fileStream = new StringReader(xml))
            {
                result = (ChunkDto)serializer.Deserialize(fileStream);
            }

            // Assert
            result.XmlChunkId.Should().Be(expectedChunkId);
            result.Sentences.Should().HaveCount(expectedNumberOfSentences);
        }

        [Theory]
        [InlineData(2, @"<?xml version=""1.0"" encoding=""UTF-8""?>
                    <!DOCTYPE chunkList SYSTEM ""ccl.dtd"">
                    <chunkList>
                    <chunk id=""ch1"" type=""p"">
                    <sentence id=""s1"">
                    <tok>
                        <orth>TEST1</orth>
                        <lex disamb=""1"">
                            <base>TEST11</base>
                            <ctag>TEST111</ctag>
                        </lex>
                    </tok>
                    <ns/>
                    <tok>
                        <orth>TEST2</orth>
                        <lex disamb=""1"">
                            <base>TEST22</base>
                            <ctag>TEST222</ctag>
                        </lex>
                    </tok>
                    </sentence>
                    </chunk>
                    <chunk id=""ch2"" type=""p"">
                    <sentence id=""s1"">
                    <tok>
                        <orth>TEST1</orth>
                        <lex disamb=""1"">
                            <base>TEST11</base>
                            <ctag>TEST111</ctag>
                        </lex>
                    </tok>
                    </sentence>
                    </chunk>
                    </chunkList>")]
        public void Corpus_ReadXml_ShouldDeserializeXMLWithoutCorpusMetadata(int expectedNumberOfChunks, string xml)
        {
            // Arrange
            XmlSerializer serializer = new XmlSerializer(typeof(ChunkListDto));
            ChunkListDto result;

            // Act
            using (StringReader fileStream = new StringReader(xml))
            {
                result = (ChunkListDto)serializer.Deserialize(fileStream);
            }

            // Assert
            // result.Id.Should().NotBe(Guid.Empty);
            result.Chunks.Should().HaveCount(expectedNumberOfChunks);
        }
    }
}