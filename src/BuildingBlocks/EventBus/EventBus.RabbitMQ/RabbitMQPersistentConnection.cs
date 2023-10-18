using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Net.Sockets;

namespace EventBus.RabbitMQ
{
    public class RabbitMQPersistentConnection : IDisposable
    {
        IConnectionFactory connectionFactory;
        private readonly int retryCount;
        private IConnection connection;
        private object lock_object = new object();
        private bool _dispose; 

        public RabbitMQPersistentConnection(IConnectionFactory connectionFactory, int retryCount = 5)
        {
            this.connectionFactory = connectionFactory;
            this.retryCount = retryCount;
        }
        public bool isConnection => connection != null && connection.IsOpen; // Bu 2 şartı sağlıyorsa true dönebilir problem yok.

        public IModel CreateModel()
        {
            return connection.CreateModel();
        }
        public void Dispose()
        {
            _dispose = true; // bunun amacı aşağıda yapılan Retry mekanizmaları çalıştığında dispose edilip edilmemsinin kontolünü yapıyoruz.
            connection.Dispose();
        }
        public bool TryConnect() // Bu yapı Retry mekanizması kurmaya yarıyor.
        {
            // Kaç kere deneyeceği parametre olarak verildi bu yüzden recursive bir şey olmayacak ve sonsuza kadar gitmeyecek. Sınırlı bir tekrar etme mekanizması oluşturuldu.

            lock (lock_object)
            {
                var policy = Policy.Handle<SocketException>()
                      .Or<BrokerUnreachableException>()
                      .WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), (ex, time) =>
                      {
                      }

                    );

                policy.Execute(() =>
                {
                    connection = connectionFactory.CreateConnection();
                });


                if (isConnection)
                {
                    connection.ConnectionShutdown += Connection_ConnectionShutdown;
                    connection.CallbackException += Connection_CallbackException;
                    connection.ConnectionBlocked += Connection_ConnectionBlocked;
                    //log
                    return true;
                }
                return false;
            }
        }

        private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            if (_dispose) return; // Eğer ki dispose edildiyse denemesin ve geri gitsin diye yazdık.
            // log Connection_ConnectionShutdown
            TryConnect(); // Tekrar TryConnect yapısını çağırıp tekrar denemesi için
        }
        private void Connection_ConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (_dispose) return;
            TryConnect();
        }
        private void Connection_CallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (_dispose) return;
            TryConnect();
        }
    }
}
