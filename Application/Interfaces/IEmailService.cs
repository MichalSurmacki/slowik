using System;

namespace Application.Interfaces
{
    public interface IEmailService
    {
        public void SendCorpusGuidViaEmail(string emailTo, Guid corpusId);
    }
}