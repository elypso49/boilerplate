using Microsoft.Extensions.Options;
using MongoDB.Bson.Serialization.Attributes;

namespace Api.Common.Options;

public abstract class BaseOptions<T> : IOptions<T>
    where T : class
{
    [BsonIgnore] public T Value => (this as T)!;
}