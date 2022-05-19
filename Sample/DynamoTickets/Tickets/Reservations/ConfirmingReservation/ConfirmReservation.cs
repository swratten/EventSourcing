using Core.Commands;
using Core.DynamoDbEventStore.Events;
using Core.DynamoDbEventStore.Repository;
using MediatR;

namespace Tickets.Reservations.ConfirmingReservation;

public record ConfirmReservation(
    Guid ReservationId
): ICommand
{
    public static ConfirmReservation Create(Guid? reservationId)
    {
        if (!reservationId.HasValue)
            throw new ArgumentNullException(nameof(reservationId));

        return new ConfirmReservation(reservationId.Value);
    }
}

internal class HandleConfirmReservation:
    ICommandHandler<ConfirmReservation>
{
    private readonly IDynamoDBRepository<Reservation> repository;
    private readonly IDynamoDBAppendScope scope;

    public HandleConfirmReservation(
        IDynamoDBRepository<Reservation> repository,
        IDynamoDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }

    public async Task<Unit> Handle(ConfirmReservation command, CancellationToken cancellationToken)
    {
        await scope.Do((expectedVersion, traceMetadata) =>
            repository.GetAndUpdate(
                command.ReservationId,
                payment => payment.Confirm(),
                expectedVersion,
                traceMetadata,
                cancellationToken
            )
        );

        return Unit.Value;
    }
}
