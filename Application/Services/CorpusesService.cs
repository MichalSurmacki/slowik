using System.IO;
using System.Xml.Serialization;
using Application.Dtos;
using Application.Dtos.Temporary;
using Application.Interfaces;

namespace Application.Services
{
    //this class realizes operations - reading, searching etc. on corpuses
    public class CorpusesService : ICorpusesService
    {
        public CorpusMetaData CorpusMetaData { get; set; }

        public Corpus ParseCCLFileToObject(string path)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Corpus));
            Corpus result;
            using (FileStream fileStream = new FileStream(path, FileMode.Open))
            {
                result = (Corpus)serializer.Deserialize(fileStream);
            }
            return result;
        }
    }
}