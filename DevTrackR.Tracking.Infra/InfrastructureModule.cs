using DevTrackR.Tracking.Core.Repositories;
using DevTrackR.Tracking.Infra.Messaging;
using DevTrackR.Tracking.Infra.Persistance.Repositories;
using DevTrackR.Tracking.Infra.Persistance;
using MongoDB.Driver;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using Microsoft.Extensions.Configuration;

namespace DevTrackR.Tracking.Infra
{
    public static class InfrastructureModule
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services
                .AddMongo()
                .AddRepositories()
                .AddMessageBus();

            return services;
        }

        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            services.AddSingleton<MongoDbOptions>(sp => {
                var configuration = sp.GetService<IConfiguration>();
                var options = new MongoDbOptions();

                configuration.GetSection("Mongo").Bind(options);

                return options;
            });

            services.AddSingleton<IMongoClient>(sp => {
                var configuration = sp.GetService<IConfiguration>();
                var options = sp.GetService<MongoDbOptions>();

                var client = new MongoClient(options.ConnectionString);
                var db = client.GetDatabase(options.Database);

                return client;
            });

            services.AddTransient(sp => {
                BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;

                var options = sp.GetService<MongoDbOptions>();
                var mongoClient = sp.GetService<IMongoClient>();

                var db = mongoClient.GetDatabase(options.Database);

                return db;
            });

            return services;
        }

        private static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IShippingOrderUpdateRepository, ShippingOrderUpdateRepository>();

            return services;
        }

        private static IServiceCollection AddMessageBus(this IServiceCollection services)
        {
            services.AddScoped<IMessageBusService, RabbitMqService>();

            return services;
        }
    }
}
