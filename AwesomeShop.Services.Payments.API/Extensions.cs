using AwesomeShop.Services.Payments.API.Infrastructure;
using AwesomeShop.Services.Payments.API.Infrastructure.Repositories;
using AwesomeShop.Services.Payments.API.Subscribers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace AwesomeShop.Services.Payments.API;

public static class Extensions
{
    public static IServiceCollection AddPaymentGateway(this IServiceCollection services) {
        services.AddTransient<IPaymentGatewayService, PaymentGatewayService>();

        return services;
    }

    public static IServiceCollection AddSubscribers(this IServiceCollection services) {
        services.AddSingleton(s => {
            var configuration = s.GetService<IConfiguration>();
            ArgumentNullException.ThrowIfNull(configuration);

            var rabbitMQConfig = configuration.GetSection("RabbitMQ").Get<RabbitMQOptions>();
            ArgumentNullException.ThrowIfNull(rabbitMQConfig);

            return rabbitMQConfig;
        });
        services.AddHostedService<OrderCreatedSubscriber>();
        
        return services;
    }

    public static IServiceCollection AddMongo(this IServiceCollection services) {
        BsonSerializer.RegisterSerializationProvider(new GuidSerializerProvider());
        #pragma warning disable CS0618
        BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
        BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V3;
        #pragma warning restore CS0618

        services.AddSingleton(s => {
            var configuration = s.GetService<IConfiguration>();
            ArgumentNullException.ThrowIfNull(configuration);

            var mongoConfig = configuration.GetSection("Mongo").Get<MongoDBOptions>();
            ArgumentNullException.ThrowIfNull(mongoConfig);

            return mongoConfig;
        });

        services.AddSingleton<IMongoClient>(sp => {
            var options = sp.GetService<MongoDBOptions>();
            ArgumentNullException.ThrowIfNull(options);
            
            return new MongoClient(options.ConnectionStrings);
        });

        services.AddTransient(sp => {
            var options = sp.GetService<MongoDBOptions>();
            ArgumentNullException.ThrowIfNull(options);
            var mongoClient = sp.GetService<IMongoClient>();
            ArgumentNullException.ThrowIfNull(mongoClient);

            return mongoClient.GetDatabase(options.Database);
        });

        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services) {
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        
        return services;
    }

}