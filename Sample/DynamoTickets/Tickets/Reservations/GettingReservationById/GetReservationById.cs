using Core.Exceptions;
using Core.Queries;
using EfficientDynamoDb;
using Core.DynamoDbEventStore.Repository;

namespace Tickets.Reservations.GettingReservationById;

public record GetReservationById(
    Guid ReservationId
): IQuery<ReservationDetails>;


internal class HandleGetReservationById :
    IQueryHandler<GetReservationById, ReservationDetails>
{
    private readonly DynamoDbContext context;

    public HandleGetReservationById(DynamoDbContext context)
    {
        this.context = context;
    }

    public async Task<ReservationDetails> Handle(GetReservationById request, CancellationToken cancellationToken)
    {
       //return await DynamoDBRepositoryExtensions.Get<Reservation>(repository, request.ReservationId, cancellationToken);
        //For Projections
        return await context.GetItemAsync<ReservationDetails>(request.ReservationId, cancellationToken)
              ?? throw AggregateNotFoundException.For<ReservationDetails>(request.ReservationId);
    }
}