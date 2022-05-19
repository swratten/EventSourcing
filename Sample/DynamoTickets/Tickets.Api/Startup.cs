using System.Net;
using Core;
using Core.Exceptions;
using Core.DynamoDbEventStore.OptimisticConcurrency;
using Core.DynamoDbEventStore;
using Core.Serialization.Newtonsoft;
using Core.WebApi.Middlewares.ExceptionHandling;
using Core.WebApi.OptimisticConcurrency;
using Core.WebApi.Swagger;
using Core.WebApi.Tracing;
using Microsoft.OpenApi.Models;

namespace Tickets.Api;

public class Startup
{
    private readonly IConfiguration config;

    public Startup(IConfiguration config)
    {
        this.config = config;
    }

    public void ConfigureServices(IServiceCollection services)
    {

        services.AddMvc()
            .AddNewtonsoftJson(opt => opt.SerializerSettings.WithDefaults());

        services.AddControllers();

        services
            .AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tickets", Version = "v1" });
                c.OperationFilter<MetadataOperationFilter>();
            })
            .AddCoreServices()
            .AddDynamoDBStreamSubscription("write_events")
            .AddTicketsModule(config)
            .AddCorrelationIdMiddleware()
            .AddOptimisticConcurrencyMiddleware(
                sp => sp.GetRequiredService<DynamoDBExpectedStreamVersionProvider>().TrySet,
                sp => () => sp.GetRequiredService<DynamoDBNextStreamVersionProvider>().Value?.ToString()
            );
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {

        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseExceptionHandlingMiddleware(exception => exception switch
            {
                AggregateNotFoundException _ => HttpStatusCode.NotFound,
                _ => HttpStatusCode.InternalServerError
            })
            .UseCorrelationIdMiddleware()
            .UseOptimisticConcurrencyMiddleware()
            .UseRouting()
            .UseAuthorization()
            .UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            })
            .UseSwagger()
            .UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tickets V1");
                c.RoutePrefix = string.Empty;
            });
    }
}
