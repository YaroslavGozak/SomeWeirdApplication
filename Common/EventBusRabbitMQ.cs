using Common.SubscriptionManager;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        private readonly string _connectionString;
        private readonly string _brokerName = "testing_event_bus";
        private IEventBusSubscriptionManager _subsManager;
        private IConnection _persistentConnection;
        private IModel _consumerChannel;
        private IServiceProvider _services;
        private readonly string _queueName;
        
        public EventBusRabbitMQ(IEventBusSubscriptionManager subsManager, IServiceProvider services)
        {
            _subsManager = subsManager;
            _services = services;
            _consumerChannel = CreateConsumerChannel();

            var factory = new ConnectionFactory()
            {
                HostName = _connectionString
            };

            _persistentConnection = factory.CreateConnection();
        }

        private IModel CreateConsumerChannel()
        {
            var channel = _persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: _brokerName, type: "direct");
            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            channel.CallbackException += (sender, ea) =>
            {
                _consumerChannel.Dispose();
                _consumerChannel = CreateConsumerChannel();
                StartBasicConsume();
            };

            return channel;
        }

        private void StartBasicConsume()
        {
            if(_consumerChannel != null)
            {
                var consumer = new AsyncEventingBasicConsumer(_consumerChannel);
                consumer.Received += ConsumerReceived;

                _consumerChannel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
            }
        }

        private async Task ConsumerReceived(object sender, BasicDeliverEventArgs @event)
        {
            var eventName = @event.RoutingKey;
            var message = Encoding.UTF8.GetString(@event.Body);

            await ProcessEvent(eventName, message);

            _consumerChannel.BasicAck(@event.DeliveryTag, multiple: false);
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_subsManager.HasSubscriptionsForEvent(eventName))
            {
                var handlertypes = _subsManager.GetHandlersForEvent(eventName);
                foreach(var handlerType in handlertypes)
                {
                    var handler = _services.GetService(handlerType);
                    var eventType = _subsManager.GetEventTypeByName(eventName);
                    var integrationEvent = JsonConvert.DeserializeObject(message, eventType);
                    var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);

                    await Task.Yield();
                    await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent });
                }
            }
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
            _persistentConnection.Dispose();
        }
    }
}
