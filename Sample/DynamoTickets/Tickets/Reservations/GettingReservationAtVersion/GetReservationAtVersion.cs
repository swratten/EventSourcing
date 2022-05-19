using Core.Exceptions;
using Core.Queries;
using Core.DynamoDbEventStore.Events;
using Tickets.Reservations.GettingReservationById;
using EfficientDynamoDb;

namespace Tickets.Reservations.GettingReservationAtVersion;

public record GetReservationAtVersion(
    Guid ReservationId,
    int Version
): IQuery<ReservationDetails>
{
    public static GetReservationAtVersion Create(Guid reservationId, int version)
    {
        if (reservationId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(reservationId));
        if (version < 0)
            throw new ArgumentOutOfRangeException(nameof(version));

        return new GetReservationAtVersion(reservationId, version);
    }
}

internal class HandleGetReservationAtVersion:
    IQueryHandler<GetReservationAtVersion, ReservationDetails>
{
    private readonly DynamoDbContext querySession;

    public HandleGetReservationAtVersion(DynamoDbContext querySession)
    {
        this.querySession = querySession;
    }

    public async Task<ReservationDetails> Handle(GetReservationAtVersion query, CancellationToken cancellationToken)
    {
        var (reservationId, version) = query;
        return await querySession.AggregateStream<ReservationDetails>(
            reservationId,
            cancellationToken,
            (ulong)version
        ) ?? throw AggregateNotFoundException.For<ReservationDetails>(reservationId);
    }
}
