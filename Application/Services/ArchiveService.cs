using System;
using System.IO;
using System.IO.Compression;
using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public class ArchiveService : IArchiveService
    {
        public ZipArchive GetZipArchiveFromIFormFile(IFormFile archive)
        {
            try
            {
                return new ZipArchive(archive.OpenReadStream(), ZipArchiveMode.Read);
            }
            catch(Exception ex)
            {
                if (ex is ArgumentException || ex is ArgumentNullException || ex is ArgumentOutOfRangeException || ex is InvalidDataException)
                    return null;
                throw;
            }
        }
    }
}