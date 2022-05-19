using Core.Projections;
using EfficientDynamoDb.Attributes;
using Tickets.Reservations.CancellingReservation;
using Tickets.Reservations.ConfirmingReservation;
using Tickets.Reservations.CreatingTentativeReservation;

namespace Tickets.Reservations.GettingReservations;

[DynamoDbTable("reservation_short_info")]
public class ReservationShortInfo : IVersionedProjection
{
    [DynamoDbProperty("id", DynamoDbAttributeType.PartitionKey)]
    public Guid Id { get; set; }
    [DynamoDbProperty("number")]
    public string Number { get; set; } = default!;
    [DynamoDbProperty("status")]
    public ReservationStatus Status { get; set; }
    [DynamoDbProperty("last_processed_version")]
    public ulong LastProcessedPosition { get; set; }

    public void Apply(TentativeReservationCreated @event)
    {
        Id = @event.ReservationId;
        Number = @event.Number;
        Status = ReservationStatus.Tentative;
    }

    public void Apply(ReservationConfirmed @event)
    {
        Status = ReservationStatus.Confirmed;
    }

    public void Apply(ReservationCancelled @event)
    {
        Status = ReservationStatus.Cancelled;
    }

    public void When(object @event)
    {
        switch (@event)
        {
            case TentativeReservationCreated tentativeReservationCreated:
                Apply(tentativeReservationCreated);
                return;
            case ReservationConfirmed reservationConfirmed:
                Apply(reservationConfirmed);
                return;
            case ReservationCancelled reservationCancelled:
                Apply(reservationCancelled);
                return;
        }
    }
}

// public class ReservationShortInfoProjection : AggregateProjection<ReservationShortInfo>
// {
//     public ReservationShortInfoProjection()
//     {
//         ProjectEvent<TentativeReservationCreated>((item, @event) => item.Apply(@event));

//         ProjectEvent<ReservationConfirmed>((item, @event) => item.Apply(@event));

//         DeleteEvent<ReservationCancelled>();
//     }
// }