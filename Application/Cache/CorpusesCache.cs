using Microsoft.Extensions.Caching.Memory;

namespace Application.Cache
{
    public class CorpusesCache
    {
        public MemoryCache Cache { get; set; }
        public CorpusesCache()
        {
            Cache = new MemoryCache(new MemoryCacheOptions{SizeLimit = 10});    //10 corpuses in cashe
        }
    }
}