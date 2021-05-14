namespace Application.Dtos.Clarin
{
    public class TaskStatusDto
    {
        public string Status { get; set; }
        public string ResultFileId { get; set; } = null;
        public string ErrorMessage { get; set; } = null;
        public string ProcessingValue { get; set; } = null;
        public bool UnknowStatus { get; set; } = false;
    }
}