using Confluent.Kafka;
using Orchestrator.Features.Orders.Domain;
using Orchestrator.Features.Orders.Handlers;
using Orchestrator.Serialization;

namespace Orchestrator.Features.Orders.Consumers;

internal static class OrderRegisteredConsumer
{
    internal sealed class Consumer(ConsumerConfig consumerConfig, OrderService.IService orderService)
        : BackgroundService
    {
        private readonly IConsumer<string, OrderRegisteredEventHandler.Event> _consumer =
            new ConsumerBuilder<string, OrderRegisteredEventHandler.Event>(consumerConfig)
                .SetValueDeserializer(new CustomJsonSerializer<OrderRegisteredEventHandler.Event>())
                .Build();

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

                    var response = await orderService.CreateAsync(request, stoppingToken);

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