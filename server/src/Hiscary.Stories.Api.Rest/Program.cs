using Hiscary.ServiceDefaults;
using Hiscary.Shared.Api.Rest;
using Hiscary.Stories.Api.Rest.Endpoints;
using Hiscary.Stories.Application.Read;
using Hiscary.Stories.Application.Write;
using Hiscary.Stories.EventHandlers;
using Hiscary.Stories.Jobs;
using Hiscary.Stories.Persistence.Context;
using Hiscary.Stories.Persistence.Read;
using Hiscary.Stories.Persistence.Write;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackNucleus.DDD.Api.Rest.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSharedRestApi(builder.Configuration);

builder.Services.AddStoriesPersistenceContext(builder.Configuration);
builder.Services.AddStoriesPersistenceWriteLayer();
builder.Services.AddStoriesPersistenceReadLayer();
builder.Services.AddStoriesApplicationWriteLayer();
builder.Services.AddStoriesApplicationReadLayer();
builder.Services.AddJobs();
builder.AddEventHandlers(builder.Configuration);
builder.Services.AddSerilog();
builder.Services.AddLogging();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddControllers(options => { options.Filters.Add<ValidationFilter>(); })
    .AddJsonOptions(jsonOptions =>
    {
        jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

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
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI();

    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<StoriesContext>();
        context.Database.Migrate();
    }
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapStoriesEndpoints();

app.Run();