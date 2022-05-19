using Core.Commands;
using Core.DynamoDbEventStore.Events;
using Core.DynamoDbEventStore.Repository;
using MediatR;
using Tickets.Reservations.NumberGeneration;

namespace Tickets.Reservations.CreatingTentativeReservation;

public record CreateTentativeReservation(
    Guid ReservationId,
    Guid SeatId
    ) : ICommand
{
    public static CreateTentativeReservation Create(Guid? reservationId, Guid? seatId)
    {
        if (!reservationId.HasValue)
            throw new ArgumentNullException(nameof(reservationId));
        if (!seatId.HasValue)
            throw new ArgumentNullException(nameof(seatId));

        return new CreateTentativeReservation(reservationId.Value, seatId.Value);
    }
}

internal class HandleCreateTentativeReservation:
    ICommandHandler<CreateTentativeReservation>
{
    private readonly IDynamoDBRepository<Reservation> repository;
    private readonly IReservationNumberGenerator reservationNumberGenerator;
    private readonly IDynamoDBAppendScope scope;

    public HandleCreateTentativeReservation(
        IDynamoDBRepository<Reservation> repository,
        IReservationNumberGenerator reservationNumberGenerator,
        IDynamoDBAppendScope scope
    )
    {
        this.repository = repository;
        this.reservationNumberGenerator = reservationNumberGenerator;
        this.scope = scope;
    }

    public async Task<Unit> Handle(CreateTentativeReservation command, CancellationToken cancellationToken)
    {
        var (reservationId, seatId) = command;

        await scope.Do((_, eventMetadata) =>
            repository.Add(
                Reservation.CreateTentative(
                    reservationId,
                    reservationNumberGenerator,
                    seatId
                ),
                eventMetadata,
                cancellationToken
            )
        );
        return Unit.Value;
    }
}
