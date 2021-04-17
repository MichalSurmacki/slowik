using Microsoft.Extensions.Caching.Memory;


namespace Api.CasheSettings
{
    public class CorpusCashe 
    {
        public MemoryCache Cache { get; set; }
        public CorpusCashe()
        {
            Cache = new MemoryCache(new MemoryCacheOptions{SizeLimit = 100});    //100 corpusses in cashe
        }
    }
}
