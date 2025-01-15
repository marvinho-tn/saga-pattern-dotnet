using Common.Serialization;
using Confluent.Kafka;
using Worker.Domain;

namespace Worker.Consumers;

internal static class OrderRegisteredConsumer
{
    internal record Message(string ProductId, int Quantity);

    internal sealed class Consumer : BackgroundService
    {
        private readonly OrderService.Service _orderService;
        private readonly IConsumer<string, Message> _consumer;

        public Consumer(ConsumerConfig consumerConfig, OrderService.Service orderService)
        {
            _orderService = orderService;

            consumerConfig.GroupId = "order-group";

            _consumer = new ConsumerBuilder<string, Message>(consumerConfig)
                .SetValueDeserializer(new CustomJsonSerializer<Message>())
                .Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe("order-registered");

            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(stoppingToken);

                if (consumeResult.Message.Value is not null)
                {
                    var message = consumeResult.Message.Value;
                    var request = new OrderService.Request(message.ProductId, message.Quantity);

                    var response = await _orderService.CreateAsync(request, stoppingToken);

                    if (response.IsSuccessResponse)
                    {
                        _consumer.Commit(consumeResult);
                    }
                }
            }
        }

        public override void Dispose()
        {
            _consumer.Close();
            _consumer.Dispose();
        }
    }
}