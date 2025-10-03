using Hiscary.Recommendations.Api.Rest.Endpoints;
using Hiscary.Recommendations.Application.Read;
using Hiscary.Recommendations.Application.Write;
using Hiscary.Recommendations.Persistence.Read;
using Hiscary.Recommendations.Persistence.Write;
using Hiscary.ServiceDefaults;
using Hiscary.Shared.Application.Extensions;
using Hiscary.Shared.Domain.Options;
using Serilog;
using StackNucleus.DDD.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSerilog();
builder.Services.AddLogging();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddBoundSettingsWithSectionAsEntityName<JwtSettings>(builder.Configuration, out var jwtSettings);
builder.Services.AddHttpContextAccessor();
builder.Services.AddJwtBearerSupport(jwtSettings);

builder.Services.AddRecommendationsPersistenceWriteLayer();
builder.Services.AddRecommendationsPersistenceReadLayer();

builder.Services.AddRecommendationsApplicationReadLayer();
builder.Services.AddRecommendationsApplicationWriteLayer();

builder.AddElasticsearchClient(connectionName: "elasticsearch");

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.DictionaryKeyPolicy = null;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
        policy.AllowAnyOrigin();
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapRecommendationsEndpoints();

app.Run();
