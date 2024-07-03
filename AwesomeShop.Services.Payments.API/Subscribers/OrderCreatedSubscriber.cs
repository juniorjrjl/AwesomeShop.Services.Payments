using System.Text;
using AwesomeShop.Services.Payments.API.Core.Entities;
using AwesomeShop.Services.Payments.API.Events;
using AwesomeShop.Services.Payments.API.Infrastructure;
using AwesomeShop.Services.Payments.API.Infrastructure.DTOs;
using AwesomeShop.Services.Payments.API.Infrastructure.Repositories;
using AwesomeShop.Services.Payments.API.Subscribers.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AwesomeShop.Services.Payments.API.Subscribers
{

    public class OrderCreatedSubscriber : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private const string Queue = "payment-service/order-created";
        private const string Exchange = "payment-service";
        public OrderCreatedSubscriber(IServiceProvider serviceProvider, RabbitMQOptions rabbitMQOptions)
        {
            _serviceProvider = serviceProvider;

            var connectionFactory = new ConnectionFactory {
                HostName = rabbitMQOptions.Host,
                UserName = rabbitMQOptions.User,
                Password = rabbitMQOptions.Password,
                Port = rabbitMQOptions.Port,
                VirtualHost = rabbitMQOptions.VirtualHost
            };

            _connection = connectionFactory.CreateConnection("payment-service-order-created-consumer"); 

            _channel = _connection.CreateModel();
            
            _channel.ExchangeDeclare(Exchange, ExchangeType.Topic, durable: true);
            _channel.QueueDeclare(Queue, durable: false, exclusive: false, autoDelete: false, arguments: null);
            _channel.QueueBind(Queue, Exchange, Queue);
            
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (sender, eventArgs) => {
                var contentArray = eventArgs.Body.ToArray();
                var contentString = Encoding.UTF8.GetString(contentArray);
                var message = JsonConvert.DeserializeObject<OrderCreated>(contentString);
                ArgumentNullException.ThrowIfNull(message);
                ArgumentNullException.ThrowIfNull(message.Id);

                Console.WriteLine($"Message OrderCreated received with Id {message.Id}");

                var result = await ProcessPayment(message);

                if (result) {
                    _channel.BasicAck(eventArgs.DeliveryTag, false);

                    var paymentAccepted = new PaymentAccepted(message.Id, message.FullName, message.Email);
                    var payload = JsonConvert.SerializeObject(paymentAccepted);
                    var byteArray = Encoding.UTF8.GetBytes(payload);

                    Console.WriteLine("PaymentAccepted Published");
                    
                    _channel.BasicPublish(Exchange, "payment-accepted", null, byteArray);
                }
            };

            _channel.BasicConsume(Queue, false, consumer);

            return Task.CompletedTask;
        }

        private async Task<bool> ProcessPayment(OrderCreated orderCreated) {
            using var scope = _serviceProvider.CreateScope();
            var paymentService = scope.ServiceProvider.GetService<IPaymentGatewayService>();
            ArgumentNullException.ThrowIfNull(paymentService);

            var result = await paymentService
                .Process(new CreditCardInfo(
                    orderCreated.PaymentInfo.CardNumber,
                    orderCreated.PaymentInfo.FullName,
                    orderCreated.PaymentInfo.ExpirationDate,
                    orderCreated.PaymentInfo.Cvv));

            var invoiceRepository = scope.ServiceProvider.GetService<IInvoiceRepository>();
            ArgumentNullException.ThrowIfNull(invoiceRepository);

            await invoiceRepository.AddAsync(new Invoice(orderCreated.TotalPrice, orderCreated.Id, orderCreated.PaymentInfo.CardNumber));

            return result;
        }
    }

}
