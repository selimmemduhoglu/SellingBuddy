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
            services = new ServiceCollection(); //bunun kulaln�lma amac� ServiceProvider elde etmek i�in
            services.AddLogging(configure => configure.AddConsole());
        }

        [TestMethod]
        public void subscribe_event_on_rabbitmq_test()
        {
            //Buras� �ey demek senden ne zaman ki   IEventBus Interface ile ile bir�ey istenirse git return EventBusFactory.Create(config, sp); bu i�lemi yap demek. (yani Create etti�i i�in Service Bus � aya�a kald�rm�� oldu.)
            services.AddSingleton<IEventBus>(sp =>
            {
                EventBusConfig config = new()
                {
                    ConnectionRetryCount = 5,
                    SubscriptionClientAppName = "EventBus.UnitTest",
                    DefaultTopicName = "SellingBuddyTopicName",
                    EventBusType = EventBusType.RabbitMQ, //Burada hangi EventBus � istedi�imizi s�yledik
                    EventNameSuffix = "IntegrationEvent",
                };

                return EventBusFactory.Create(config, sp);

            });

            var sp = services.BuildServiceProvider();

            var eventBus = sp.GetRequiredService<IEventBus>();

            eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
            // Bir istek geldi�i zaman, bir mesaj geldi�i zaman OrderCreatedIntegrationEvent ' e git OrderCreatedIntegrationEventHandler ' � kullan demi� olduk.

            eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

        }
    }
}