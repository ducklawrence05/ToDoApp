using ToDoApp.Application.Services;

namespace ToDoApp.Application.Dtos
{
    public class GuidData
    {
        public IGuidGenerator guidGenerator { get; set; }

        public Guid GetGuid()
        {
            return guidGenerator.Generate();
        }
    }
}
