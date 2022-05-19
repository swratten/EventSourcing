using Core.Commands;
using Core.DynamoDbEventStore.Repository;
using Core.DynamoDbEventStore.ExternalProjections;
using Core.Queries;
using Microsoft.Extensions.DependencyInjection;
using Tickets.Reservations.CancellingReservation;
using Tickets.Reservations.ChangingReservationSeat;
using Tickets.Reservations.ConfirmingReservation;
using Tickets.Reservations.CreatingTentativeReservation;
using Tickets.Reservations.GettingReservationAtVersion;
using Tickets.Reservations.GettingReservationById;
using Tickets.Reservations.GettingReservationHistory;
using Tickets.Reservations.GettingReservations;
using Tickets.Reservations.NumberGeneration;

namespace Tickets.Reservations;

internal static class ReservationsConfig
{
    internal static IServiceCollection AddReservations(this IServiceCollection services) =>
        services
            .AddScoped<IReservationNumberGenerator, ReservationNumberGenerator>()
            .AddScoped<IDynamoDBRepository<Reservation>, DynamoDBRepository<Reservation>>()
            .AddCommandHandlers()
            .AddQueryHandlers()
            .AddProjections();

    private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services
            .AddCommandHandler<CreateTentativeReservation, HandleCreateTentativeReservation>()
            .AddCommandHandler<ChangeReservationSeat,HandleChangeReservationSeat>()
            .AddCommandHandler<ConfirmReservation, HandleConfirmReservation>()
            .AddCommandHandler<CancelReservation, HandleCancelReservation>();

    private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
        services
            .AddQueryHandler<GetReservationById, ReservationDetails, HandleGetReservationById>()
            .AddQueryHandler<GetReservationAtVersion, ReservationDetails, HandleGetReservationAtVersion>()
            .AddQueryHandler<GetReservations, IReadOnlyList<ReservationShortInfo>, HandleGetReservations>()
            .AddQueryHandler<GetReservationHistory, IReadOnlyList<ReservationHistory>, HandleGetReservationHistory>();

    private static IServiceCollection AddProjections(this IServiceCollection services)
    {
        services
            .Project<TentativeReservationCreated, ReservationDetails>(@event => @event.ReservationId)
            .Project<ReservationSeatChanged, ReservationDetails>(@event => @event.ReservationId)
            .Project<ReservationConfirmed, ReservationDetails>(@event => @event.ReservationId)
            .Project<ReservationCancelled, ReservationDetails>(@event => @event.ReservationId);

        services
            .Project<TentativeReservationCreated, ReservationShortInfo>(@event => @event.ReservationId)
            .Project<ReservationSeatChanged, ReservationShortInfo>(@event => @event.ReservationId)
            .Project<ReservationConfirmed, ReservationShortInfo>(@event => @event.ReservationId)
            .Project<ReservationCancelled, ReservationShortInfo>(@event => @event.ReservationId);



        // services
        //     .Project<ShoppingCartInitialized, CartHistory>(@event => @event.CartId)
        //     .Project<ProductAdded, CartHistory>(@event => @event.CartId)
        //     .Project<ProductRemoved, CartHistory>(@event => @event.CartId)
        //     .Project<ShoppingCartConfirmed, CartHistory>(@event => @event.CartId);

        return services;
    }
    // internal static void ConfigureReservations(this StoreOptions options)
    // {
        // // Snapshots
        // options.Projections.SelfAggregate<Reservation>();
        // options.Schema.For<Reservation>().Index(x => x.SeatId, x =>
        // {
        //     x.IsUnique = true;

        //     // Partial index by supplying a condition
        //     x.Predicate = "(data ->> 'Status') != 'Cancelled'";
        // });
        // options.Schema.For<Reservation>().Index(x => x.Number, x =>
        // {
        //     x.IsUnique = true;

        //     // Partial index by supplying a condition
        //     x.Predicate = "(data ->> 'Status') != 'Cancelled'";
        // });

        // // options.Schema.For<Reservation>().UniqueIndex(x => x.SeatId);

        // // projections
        // options.Projections.Add<ReservationDetailsProjection>();
        // options.Projections.Add<ReservationShortInfoProjection>();

        // // transformation
        // options.Projections.Add<ReservationHistoryTransformation>();
    // }
}
