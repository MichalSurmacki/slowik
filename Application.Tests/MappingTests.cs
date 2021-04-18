using System.Collections.Generic;
using Application.Dtos.Temporary;
using AutoMapper;
using Domain.Models;
using FluentAssertions;
using Xunit;

namespace Application.Tests
{
    public class MappingTests
    {
        private readonly IConfigurationProvider _configuration;
        private readonly IMapper _mapper;
        public MappingTests()
        {
            _configuration = new MapperConfiguration( cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = _configuration.CreateMapper();
        }

        [Theory]
        [InlineData(1, "test1")]
        public void Map_SentenceDto_Sentence_ShouldMap(int id, string testXml)
        {
            // Arange
            var source = new SentenceDto
            {
                XmlSentenceId = id,
                Xml = testXml,
                Tokens = new List<TokenDto>()
            }; 

            // Act
            Sentence destination = new Sentence();
            _mapper.Map(source, destination);

            // Assert
            destination.XmlSentenceId.Should().Be(id);
            destination.Xml.Should().Be(testXml);
        }
    }
}