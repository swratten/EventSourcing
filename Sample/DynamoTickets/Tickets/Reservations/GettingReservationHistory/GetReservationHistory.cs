using Core.Queries;
using EfficientDynamoDb;

namespace Tickets.Reservations.GettingReservationHistory;

public record GetReservationHistory(
    Guid ReservationId,
    int PageNumber,
    int PageSize
): IQuery<IReadOnlyList<ReservationHistory>>
{
    public static GetReservationHistory Create(Guid reservationId, int pageNumber = 1, int pageSize = 20)
    {
        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));
        if (pageSize is <= 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        return new GetReservationHistory(reservationId, pageNumber, pageSize);
    }
}

internal class HandleGetReservationHistory:
    IQueryHandler<GetReservationHistory, IReadOnlyList<ReservationHistory>>
{
    private readonly DynamoDbContext querySession;

    public HandleGetReservationHistory(DynamoDbContext querySession)
    {
        this.querySession = querySession;
    }

    public Task<IReadOnlyList<ReservationHistory>> Handle(
        GetReservationHistory query,
        CancellationToken cancellationToken
    )
    {
        var (reservationId, pageNumber, pageSize) = query;

        return querySession.Query<ReservationHistory>()
            .WithKeyExpression(Condition.ForEntity<ReservationHistory>().On( x => x.ReservationId).EqualTo(reservationId))
            .ToListAsync();

            // .Where(h => h.ReservationId == reservationId)
            // .ToPagedListAsync(pageNumber, pageSize, cancellationToken);
    }
}
