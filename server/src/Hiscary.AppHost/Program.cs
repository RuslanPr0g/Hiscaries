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
                .WithArgs(
                "azurite",
                "-l",
                "/data",
                "--blobHost",
                "0.0.0.0",
                "--queueHost",
                "0.0.0.0",
                "--tableHost",
                "0.0.0.0",
                "--debug",
                "path/debug.log")
                .WithBindMount(
                        source: "../azurite-data",
                        target: "/data",
                        isReadOnly: false)
    )
    .AddBlobs("azblobs");

var elasticsearch = builder
    .AddElasticsearch("elasticsearch")
    .WithEnvironment("xpack.security.enabled", "false");
var redis = builder
    .AddRedis("rediscache");

var useraccounts = builder.AddProject<Projects.Hiscary_UserAccounts_Api_Rest>("hc-useraccounts-api-rest")
    .WithJwtAndSaltSettings(builder.Configuration)
    .WithHttpsEndpoint(name: "rest", port: 7010, targetPort: 7010, isProxied: false)
    .WithReference(postgres)
    .WithReference(rabbitmq)
    .WithReference(azBlobs);
var notifications = builder.AddProject<Projects.Hiscary_Notifications_Api_Rest>("hc-notifications-api-rest")
    .WithJwtAndSaltSettings(builder.Configuration)
    .WithHttpsEndpoint(name: "rest", port: 7011, targetPort: 7011, isProxied: false)
    .WithReference(postgres)
    .WithReference(rabbitmq)
    .WithReference(azBlobs);
var platformusers = builder.AddProject<Projects.Hiscary_PlatformUsers_Api_Rest>("hc-platformusers-api-rest")
    .WithJwtAndSaltSettings(builder.Configuration)
    .WithHttpsEndpoint(name: "rest", port: 7012, targetPort: 7012, isProxied: false)
    .WithReference(postgres)
    .WithReference(rabbitmq)
    .WithReference(azBlobs);
var stories = builder.AddProject<Projects.Hiscary_Stories_Api_Rest>("hc-stories-api-rest")
    .WithJwtAndSaltSettings(builder.Configuration)
    .WithHttpsEndpoint(name: "rest", port: 7013, targetPort: 7013, isProxied: false)
    .WithReference(postgres)
    .WithReference(elasticsearch)
    .WithReference(rabbitmq)
    .WithReference(azBlobs);
var media = builder.AddProject<Projects.Hiscary_Media_Api_Rest>("hc-media-api-rest")
    .WithJwtAndSaltSettings(builder.Configuration)
    .WithHttpsEndpoint(name: "rest", port: 7014, targetPort: 7014, isProxied: false)
    .WithEnvironment("ServiceUrls__MediaServiceUrl", "https://localhost:5001/api/v1/media")
    .WithReference(rabbitmq)
    .WithReference(azBlobs);
var recommendations = builder.AddProject<Projects.Hiscary_Recommendations_Api_Rest>("hiscary-recommendations-api-rest")
    .WithJwtAndSaltSettings(builder.Configuration)
    .WithHttpsEndpoint(name: "rest", port: 7015, targetPort: 7015, isProxied: false)
    .WithReference(rabbitmq)
    .WithReference(elasticsearch)
    .WithReference(redis)
    .WithReference(azBlobs);

builder.AddProject<Projects.Hiscary_LocalApiGateway>("hc-localapigateway")
    .WithHttpsEndpoint(name: "apigateway", port: 5001, targetPort: 5001, isProxied: false)
    .WithReference(useraccounts)
    .WithReference(notifications)
    .WithReference(platformusers)
    .WithReference(stories)
    .WithReference(media)
    .WithReference(recommendations);

builder.Build().Run();
