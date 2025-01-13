using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Payment.Features.Payment.Domain;

internal sealed class Payment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    public required string OrderId { get; set; }
    
    public required int Amount { get; set; }
    
    public required string Status { get; set; }
}