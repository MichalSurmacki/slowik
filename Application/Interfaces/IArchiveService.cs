using System.IO.Compression;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IArchiveService
    {
        public ZipArchive GetZipArchiveFromIFormFile(IFormFile archive);

    }
}