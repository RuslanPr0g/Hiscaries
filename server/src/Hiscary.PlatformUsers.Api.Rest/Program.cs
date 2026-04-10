using Hiscary.PlatformUsers.Api.Rest.Endpoints;
using Hiscary.PlatformUsers.Application.Read;
using Hiscary.PlatformUsers.Application.Write;
using Hiscary.PlatformUsers.EventHandlers;
using Hiscary.PlatformUsers.Jobs;
using Hiscary.PlatformUsers.Persistence.Context;
using Hiscary.PlatformUsers.Persistence.Read;
using Hiscary.PlatformUsers.Persistence.Write;
using Hiscary.ServiceDefaults;
using Hiscary.Shared.Api.Rest;
using Microsoft.EntityFrameworkCore;
using Serilog;
using StackNucleus.DDD.Api.Rest.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddSharedRestApi(builder.Configuration);

builder.Services.AddPlatformUsersPersistenceContext(builder.Configuration);
builder.Services.AddPlatformUsersPersistenceWriteLayer();
builder.Services.AddPlatformUsersPersistenceReadLayer();
builder.Services.AddPlatformUsersApplicationWriteLayer();
builder.Services.AddPlatformUsersApplicationReadLayer();
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
        var context = scope.ServiceProvider.GetRequiredService<PlatformUsersContext>();
        context.Database.Migrate();
    }
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors("AllowAll");

app.UseAuthentication();

app.UseAuthorization();

app.MapPlatformUsersEndpoints();

app.Run();