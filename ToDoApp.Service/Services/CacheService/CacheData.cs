namespace ToDoApp.Application.Services.CacheService
{
    public class CacheData
    {
        public object Value { get; set; }
        public DateTime Expiration { get; set; }
        public CacheData(object value, DateTime expiration) 
        {
            Value = value;
            Expiration = expiration;
        }
    }
}
