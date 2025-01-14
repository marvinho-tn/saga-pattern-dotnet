using Confluent.Kafka;
using FastEndpoints;
using Orchestrator.Serialization;

namespace Orchestrator.Features.Orders.Handlers;

internal static class OrderRegisteredEventHandler
{
    internal record Event(string ProductId, int Quantity);
    
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
            
            await producer.ProduceAsync("order-registered", message, ct);
        }
    }
}