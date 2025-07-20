using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Service.Services.ExampleService
{
    public class TestableExampleService : ExampleService
    {
        private readonly DateTime _fixedTime;

        public TestableExampleService(DateTime fixedTime)
        {
            _fixedTime = fixedTime;
        }

        protected override DateTime GetCurrentTime()
        {
            return _fixedTime;
        }
    }

}
