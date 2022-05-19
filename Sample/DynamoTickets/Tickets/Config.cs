using Core.DynamoDbEventStore.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tickets.Maintenance;
using Tickets.Reservations;
using Core.DynamoDbEventStore;

namespace Tickets;

public static class Config
{
    public static IServiceCollection AddTicketsModule(this IServiceCollection services, IConfiguration config) =>
        services
            .AddDynamoDB(config)
            .AddReservations()
            .AddMaintainance();
}
