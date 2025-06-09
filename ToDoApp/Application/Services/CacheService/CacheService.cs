namespace ToDoApp.Application.Services.CacheService
{
    public class CacheService : ICacheService
    {
        private Dictionary<string, CacheData> _cache = new Dictionary<string, CacheData>();

        public CacheData Get(string key)
        {
            if (!_cache.ContainsKey(key))
            {
                return null;
            }

            var cacheData = _cache[key];
            if (cacheData.Expiration < DateTime.Now)
            {
                _cache.Remove(key);
                return null;
            }
            
            return cacheData;
        }

        public void Set(string key, object value, int duration)
        {
            if(value == null || string.IsNullOrEmpty(key) || duration < 0)
            {
                return;
            }

            var newCacheData = new CacheData(value, DateTime.Now.AddSeconds(duration));
            _cache[key] = newCacheData;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
