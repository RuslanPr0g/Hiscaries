var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("hiscary")
    .WithDataVolume("hiscarydbdata")
    .WithPgAdmin()
    .AddDatabase("postgres");
var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithDataVolume("hiscaryrabbitmqdata")
    .WithManagementPlugin(38231);
var azBlobs = builder.AddAzureStorage("azstorage")
    .RunAsEmulator(
        (config) =>
            config
                .WithImageTag("latest")
                .WithBindMount(
                        source: "../azurite-data",
                        target: "/data",
                        isReadOnly: false)
    )
    .AddBlobs("azblobs");

var elasticsearch = builder
    .AddElasticsearch("elasticsearch")
    .WithBindMount(
        source: "../elasticsearch-data",
        target: "/usr/share/elasticsearch/data",
        isReadOnly: false)
    .WithEnvironment("xpack.security.enabled", "false");
var redis = builder
    .AddRedis("rediscache");

var useraccounts = builder.AddProject<Projects.Hiscary_UserAccounts_Api_Rest>("hc-useraccounts-api-rest")
    .WaitFor(postgres)
    .WaitFor(rabbitmq)
    .WaitFor(elasticsearch)
    .WaitFor(azBlobs)
    .WaitFor(redis)
    .WithJwtAndSaltSettings(builder.Configuration)
    .WithHttpsEndpoint(name: "rest", port: 7010, targetPort: 7010, isProxied: false)
    .WithReference(postgres)
    .WithReference(rabbitmq)
    .WithReference(azBlobs);
var notifications = builder.AddProject<Projects.Hiscary_Notifications_Api_Rest>("hc-notifications-api-rest")
    .WaitFor(postgres)
    .WaitFor(rabbitmq)
    .WaitFor(elasticsearch)
    .WaitFor(azBlobs)
    .WaitFor(redis)
    .WithJwtAndSaltSettings(builder.Configuration)
    .WithHttpsEndpoint(name: "rest", port: 7011, targetPort: 7011, isProxied: false)
    .WithReference(postgres)
    .WithReference(rabbitmq)
    .WithReference(azBlobs);
var platformusers = builder.AddProject<Projects.Hiscary_PlatformUsers_Api_Rest>("hc-platformusers-api-rest")
    .WaitFor(postgres)
    .WaitFor(rabbitmq)
    .WaitFor(elasticsearch)
    .WaitFor(azBlobs)
    .WaitFor(redis)
    .WithJwtAndSaltSettings(builder.Configuration)
    .WithHttpsEndpoint(name: "rest", port: 7012, targetPort: 7012, isProxied: false)
    .WithReference(postgres)
    .WithReference(rabbitmq)
    .WithReference(azBlobs);
var stories = builder.AddProject<Projects.Hiscary_Stories_Api_Rest>("hc-stories-api-rest")
    .WaitFor(postgres)
    .WaitFor(rabbitmq)
    .WaitFor(elasticsearch)
    .WaitFor(azBlobs)
    .WaitFor(redis)
    .WithJwtAndSaltSettings(builder.Configuration)
    .WithHttpsEndpoint(name: "rest", port: 7013, targetPort: 7013, isProxied: false)
    .WithReference(postgres)
    .WithReference(elasticsearch)
    .WithReference(rabbitmq)
    .WithReference(azBlobs);
var media = builder.AddProject<Projects.Hiscary_Media_Api_Rest>("hc-media-api-rest")
    .WaitFor(postgres)
    .WaitFor(rabbitmq)
    .WaitFor(elasticsearch)
    .WaitFor(azBlobs)
    .WaitFor(redis)
    .WithJwtAndSaltSettings(builder.Configuration)
    .WithHttpsEndpoint(name: "rest", port: 7014, targetPort: 7014, isProxied: false)
    .WithEnvironment("ServiceUrls__MediaServiceUrl", "https://localhost:5001/api/v1/media")
    .WithReference(rabbitmq)
    .WithReference(azBlobs);
var recommendations = builder.AddProject<Projects.Hiscary_Recommendations_Api_Rest>("hc-recommendations-api-rest")
    .WaitFor(postgres)
    .WaitFor(rabbitmq)
    .WaitFor(elasticsearch)
    .WaitFor(azBlobs)
    .WaitFor(redis)
    .WithJwtAndSaltSettings(builder.Configuration)
    .WithHttpsEndpoint(name: "rest", port: 7015, targetPort: 7015, isProxied: false)
    .WithReference(rabbitmq)
    .WithReference(elasticsearch)
    .WithReference(redis)
    .WithReference(azBlobs);

var gateway = builder.AddYarp("apigateway")
                     .WithHostPort(5001)
                     .WaitFor(useraccounts)
                     .WaitFor(notifications)
                     .WaitFor(platformusers)
                     .WaitFor(stories)
                     .WaitFor(media)
                     .WaitFor(recommendations)
                     .WithConfiguration(yarp =>
                     {
                         yarp.AddRoute("/api/v1/accounts/{**catch-all}", useraccounts);
                         yarp.AddRoute("/api/v1/notifications/{**catch-all}", notifications);
                         yarp.AddRoute("/hubs/usernotifications/{**catch-all}", notifications);
                         yarp.AddRoute("/api/v1/users/{**catch-all}", platformusers);
                         yarp.AddRoute("/api/v1/stories/{**catch-all}", stories);
                         yarp.AddRoute("/api/v1/media/{**catch-all}", media);
                         yarp.AddRoute("/api/v1/recommendations/{**catch-all}", recommendations);
                     });

builder.Build().Run();
