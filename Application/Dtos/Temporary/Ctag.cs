namespace Application.Dtos.Temporary
{
    public class CTag
    {
        public int Id { get; set; }
        //tag morfosyntaktyczny w formacie NKJP (<ctag> w CCL)
        public string TagNKJP { get; set; }
    }
}