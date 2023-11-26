using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.UnitTest.Events.EventHandlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EventBus.UnitTest
{

    [TestClass]
    public class EventBusTests
    {
        private ServiceCollection services;

        public EventBusTests()
        {
            services = new ServiceCollection(); //bunun kulalnýlma amacý ServiceProvider elde etmek için
            services.AddLogging(configure => configure.AddConsole());
        }

        [TestMethod]
        public void subscribe_event_on_rabbitmq_test()
        {
            //Burasý þey demek senden ne zaman ki   IEventBus Interface ile ile birþey istenirse git return EventBusFactory.Create(config, sp); bu iþlemi yap demek. (yani Create ettiði için Service Bus ý ayaða kaldýrmýþ oldu.)
            services.AddSingleton<IEventBus>(sp =>
            {
                EventBusConfig config = new()
                {
                    ConnectionRetryCount = 5,
                    SubscriptionClientAppName = "EventBus.UnitTest",
                    DefaultTopicName = "SellingBuddyTopicName",
                    EventBusType = EventBusType.RabbitMQ, //Burada hangi EventBus ý istediðimizi söyledik
                    EventNameSuffix = "IntegrationEvent",
                };

                return EventBusFactory.Create(config, sp);

            });

            var sp = services.BuildServiceProvider();

            var eventBus = sp.GetRequiredService<IEventBus>();

            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
            // Bir istek geldiði zaman, bir mesaj geldiði zaman OrderCreatedIntegrationEvent ' e git OrderCreatedIntegrationEventHandler ' ý kullan demiþ olduk.

            eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

        }
    }
}