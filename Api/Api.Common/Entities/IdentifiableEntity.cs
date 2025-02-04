using Api.Common.Abstraction;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Api.Common.Entities;

[BsonIgnoreExtraElements]
public abstract class IdentifiableEntity : IIdentifiable
{
    [BsonId, BsonRepresentation(BsonType.ObjectId)] public string? Id { get; set; }

    public bool? Deleted { get; set; } = false;
}