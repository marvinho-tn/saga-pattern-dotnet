using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Order.Features.Order.Domain;

public sealed class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public required string ProductId { get; set; }
    public required int Quantity { get; set; }
    public required string Status { get; set; }
}