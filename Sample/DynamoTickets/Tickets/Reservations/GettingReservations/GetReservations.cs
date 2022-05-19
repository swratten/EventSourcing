using Core.Queries;
using EfficientDynamoDb;

namespace Tickets.Reservations.GettingReservations;

public record GetReservations(
    int PageNumber,
    int PageSize
): IQuery<IReadOnlyList<ReservationShortInfo>>
{
    public static GetReservations Create(int pageNumber = 1, int pageSize = 20)
    {
        if (pageNumber <= 0)
            throw new ArgumentOutOfRangeException(nameof(pageNumber));
        if (pageSize is <= 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(pageSize));

        return new GetReservations(pageNumber, pageSize);
    }
}

internal class HandleGetReservations:
    IQueryHandler<GetReservations, IReadOnlyList<ReservationShortInfo>>
{
    private readonly DynamoDbContext querySession;

    public HandleGetReservations(DynamoDbContext querySession)
    {
        this.querySession = querySession;
    }

    public async Task<IReadOnlyList<ReservationShortInfo>> Handle(
        GetReservations query,
        CancellationToken cancellationToken
    )
    {
        List<ReservationShortInfo> results = new List<ReservationShortInfo>();
        await foreach(var item in querySession.Scan<ReservationShortInfo>().ToAsyncEnumerable())
        {
            results.Add(item);
        }
        return results;
}
    }
        
