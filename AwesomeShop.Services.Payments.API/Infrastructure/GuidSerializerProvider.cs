using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace AwesomeShop.Services.Payments.API.Infrastructure;

public class GuidSerializerProvider : IBsonSerializationProvider
{

    public IBsonSerializer? GetSerializer(Type type) => type == typeof(Guid) ? new GuidSerializer(GuidRepresentation.Standard) : null;

}