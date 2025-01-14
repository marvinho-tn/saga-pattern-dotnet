using Common.Serialization;
using Confluent.Kafka;
using Orchestrator.Domain;

namespace Orchestrator.Consumers;

internal static class ReserveAbortedConsumer
{
    internal record Message(string OrderId, string ProductId, int Quantity);

    internal sealed class InventoryConsumer(ConsumerConfig consumerConfig, InventoryService.IService inventoryService)
        : BackgroundService
    {
        private readonly IConsumer<string, Message> _consumer =
            new ConsumerBuilder<string, Message>(consumerConfig)
                .SetValueDeserializer(new CustomJsonSerializer<Message>())
                .Build();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe("reserve-aborted");

            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(stoppingToken);

                if (consumeResult.Message.Value is not null)
                {
                    var message = consumeResult.Message.Value;
                    
                    var request = new InventoryService.Request(message.OrderId, message.ProductId, message.Quantity);

                    var response = await inventoryService.ReleaseAsync(request, stoppingToken);

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

    internal sealed class PaymentConsumer(ConsumerConfig consumerConfig, PaymentService.IService paymentService)
        : BackgroundService
    {
        private readonly IConsumer<string, Message> _consumer =
            new ConsumerBuilder<string, Message>(consumerConfig)
                .SetValueDeserializer(new CustomJsonSerializer<Message>())
                .Build();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe("reserve-aborted");

            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(stoppingToken);

                if (consumeResult.Message.Value is not null)
                {
                    var message = consumeResult.Message.Value;
                    
                    var response = await paymentService.CancelAsync(message.OrderId, stoppingToken);

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