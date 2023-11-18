using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EventBus.UnitTest
{

    [TestClass]
    public class EventBusTests
    {
       private ServiceCollection services;

        public EventBusTests()
        {
            services = new ServiceCollection();
            services.AddLogging();
        }

        [TestMethod]
        public void Test1()
        {

        }
    }
}