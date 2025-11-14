using Hiscary.Media.Api.Rest.Endpoints;
using Hiscary.Media.DocumentTools.PdfPig;
using Hiscary.Media.EventHandlers;
using Hiscary.Media.FileStorage;
using Hiscary.Media.Images;
using Hiscary.ServiceDefaults;
using Hiscary.Shared.Api.Rest;
using Hiscary.Shared.Domain.Options;
using Microsoft.AspNetCore.Http.Features;
using Serilog;
using StackNucleus.DDD.Application.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSharedRestApi(builder.Configuration);

builder.Services.AddMediaFileStorage();
builder.Services.AddMediaImages();
builder.AddEventHandlers(builder.Configuration);
builder.Services.AddSerilog();
builder.Services.AddLogging();

builder.Services.AddPdfDocumentTools();

builder.AddAzureBlobServiceClient("azblobs", config => config.DisableHealthChecks = true);

builder.Services.AddBoundSettingsWithSectionAsEntityName<ServiceUrls>(builder.Configuration, out var serviceUrls);
builder.Services.AddSingleton(serviceUrls);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.DictionaryKeyPolicy = null;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = long.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
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

app.MapMediaEndpoints();

app.Run();
