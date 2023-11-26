using EventBus.AzureServiceBus;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.RabbitMQ;
using System;

namespace EventBus.Factory
{
    public static class EventBusFactory
    {
        public static IEventBus Create(EventBusConfig config, IServiceProvider serviceProvider)
        {   // Hangi evenBus kullanılacaksa onun kullanılması için belirleyici static class
            // Bu kullanım güzel bir kullanım yenilikçi
            return config.EventBusType switch
            {

                EventBusType.AzureServiceBus => new EventBusServiceBus(serviceProvider, config),
                _ => new EventBusRabbitMQ(serviceProvider, config)

            };

        }

    }
}
