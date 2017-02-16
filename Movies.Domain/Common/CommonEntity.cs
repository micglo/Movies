using MongoDB.Bson.Serialization.Attributes;

namespace Movies.Domain.Common
{
    public abstract class CommonEntity : ICommonEntity
    {
        [BsonId]
        public string Id { get; set; }
    }
}