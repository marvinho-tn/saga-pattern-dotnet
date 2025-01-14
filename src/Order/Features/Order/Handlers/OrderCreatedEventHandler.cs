using Common.Serialization;
using Confluent.Kafka;
using FastEndpoints;

namespace Order.Features.Order.Handlers;

internal static class OrderCreatedEventHandler
{
    internal record Event(string OrderId);
    
    internal class Handler(ProducerConfig producerConfig) : IEventHandler<Event>
    {
        public async Task HandleAsync(Event @event, CancellationToken ct)
        {
            var producer = new ProducerBuilder<string, Event>(producerConfig)
                .SetValueSerializer(new CustomJsonSerializer<Event>())
                .Build();
            
            var message = new Message<string, Event>
            {
                Key = Guid.NewGuid().ToString(),
                Value = @event
            };
            
            await producer.ProduceAsync("order-created", message, ct);
        }
    }
}