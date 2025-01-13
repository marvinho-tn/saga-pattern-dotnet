using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Inventory.Features.Inventory.Domain;

internal sealed class Inventory
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    public required string ProductId { get; set; }
    
    public required int Stock { get; set; }
}