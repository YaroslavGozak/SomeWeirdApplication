using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        private readonly string _connectionString;
        private readonly string _brokerName;
        private dynamic _subsManager;
        private IConnection _persistentConnection;
        private readonly string _queueName;
        
        public EventBusRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _connectionString
            };

            _persistentConnection = factory.CreateConnection();
        }

        public void Publish(IntegrationEvent @event)
        {
            var eventName = @event.GetType().Name;
            var factory = new ConnectionFactory()
            {
                HostName = _connectionString
            };

            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: _brokerName, type: "direct");
                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: _brokerName, routingKey: eventName, basicProperties: null, body: body);
            }
        }

        public void Subscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = _subsManager.GetEventKey<T>();

            var containsKey = _subsManager.HasSubscriptionsForEvent(eventName);
            if (!containsKey)
            {
                using(var channel = _persistentConnection.CreateModel())
                {
                    channel.QueueBind(queue: _queueName, exchange: _brokerName, routingKey: eventName, arguments: null);
                }
            }

            _subsManager.AddSubscription<T, TH>();
        }

        public void Unsubscribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
