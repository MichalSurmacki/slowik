using Application.Dtos.Temporary;

namespace Application.Interfaces
{
    public interface ICorpusesService
    {
        Corpus ParseCCLFileToObject(string path);
    }
}