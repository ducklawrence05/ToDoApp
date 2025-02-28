namespace ToDoApp.Application.Services
{
    public interface ISingletonGenerator
    {
        Guid Generate();
    }

    public class SingletonGenerator : ISingletonGenerator
    {
        //nếu buộc phải lấy 1 hàm của Trasient ra cho Singleton thì có thể làm như phía dưới
        private readonly IServiceProvider _serviceProvider;

        public SingletonGenerator(IServiceProvider serviceProvider) 
        {
            _serviceProvider = serviceProvider;
        }

        public Guid Generate()
        {
            var guidGenerator = _serviceProvider.GetService<IGuidGenerator>();
            return guidGenerator.Generate();
        }
    }
}
