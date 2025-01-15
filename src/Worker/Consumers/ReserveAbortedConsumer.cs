using Common.Serialization;
using Confluent.Kafka;
using Worker.Domain;

namespace Worker.Consumers;

internal static class ReserveAbortedConsumer
{
    internal record Message(string OrderId, string ProductId, int Quantity);

    internal sealed class InventoryConsumer : BackgroundService
    {
        private readonly InventoryService.Service _inventoryService;
        private readonly IConsumer<string, Message> _consumer;

        public InventoryConsumer(ConsumerConfig consumerConfig, InventoryService.Service inventoryService)
        {
            _inventoryService = inventoryService;

            consumerConfig.GroupId = "inventory-group";

            _consumer = new ConsumerBuilder<string, Message>(consumerConfig)
                .SetValueDeserializer(new CustomJsonSerializer<Message>())
                .Build();
        }

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

                    var response = await _inventoryService.ReleaseAsync(request, stoppingToken);

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

    internal sealed class PaymentConsumer : BackgroundService
    {
        private readonly PaymentService.Service _paymentService;
        private readonly IConsumer<string, Message> _consumer;

        public PaymentConsumer(ConsumerConfig consumerConfig, PaymentService.Service paymentService)
        {
            _paymentService = paymentService;

            consumerConfig.GroupId = "payment-group";

            _consumer = new ConsumerBuilder<string, Message>(consumerConfig)
                .SetValueDeserializer(new CustomJsonSerializer<Message>())
                .Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe("reserve-aborted");

            while (!stoppingToken.IsCancellationRequested)
            {
                var consumeResult = _consumer.Consume(stoppingToken);

                if (consumeResult.Message.Value is not null)
                {
                    var message = consumeResult.Message.Value;
                    
                    var response = await _paymentService.CancelAsync(message.OrderId, stoppingToken);

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