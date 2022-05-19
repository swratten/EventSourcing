using Core.Projections;
using Tickets.Reservations.CancellingReservation;
using Tickets.Reservations.ChangingReservationSeat;
using Tickets.Reservations.ConfirmingReservation;
using EfficientDynamoDb.Attributes;
using Tickets.Reservations.CreatingTentativeReservation;
using System.Text.Json.Serialization;

namespace Tickets.Reservations.GettingReservationById;

[DynamoDbTable("reservation_details")]
public class ReservationDetails : IVersionedProjection
{
    [DynamoDbProperty("id", DynamoDbAttributeType.PartitionKey)]
    public Guid Id { get; set; }
    [DynamoDbProperty("number")]
    public string Number { get; set; } = default!;
    [DynamoDbProperty("seat_id")]
    public Guid SeatId { get; set; }
    [DynamoDbProperty("status")]
    public ReservationStatus Status { get; set; }
    [DynamoDbProperty("version")]
    public int Version { get; set; }
    [DynamoDbProperty("last_processed_version")]
    public ulong LastProcessedPosition { get; set; }

    public void Apply(TentativeReservationCreated @event)
    {
        Id = @event.ReservationId;
        SeatId = @event.SeatId;
        Number = @event.Number;
        Status = ReservationStatus.Tentative;
        Version++;
    }

    public void Apply(ReservationSeatChanged @event)
    {
        SeatId = @event.SeatId;
        Version++;
    }

    public void Apply(ReservationConfirmed @event)
    {
        Status = ReservationStatus.Confirmed;
        Version++;
    }

    public void Apply(ReservationCancelled @event)
    {
        Status = ReservationStatus.Cancelled;
        Version++;
    }

    public void When(object @event)
    {
        switch (@event)
        {
            case TentativeReservationCreated tentativeReservationCreated:
                Apply(tentativeReservationCreated);
                return;
            case ReservationSeatChanged reservationSeatChanged:
                Apply(reservationSeatChanged);
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

// public class ReservationDetailsProjection: AggregateProjection<ReservationDetails>
// {
//     public ReservationDetailsProjection()
//     {
//         ProjectEvent<TentativeReservationCreated>((item, @event) => item.Apply(@event));

//         ProjectEvent<ReservationSeatChanged>((item, @event) => item.Apply(@event));

//         ProjectEvent<ReservationConfirmed>((item, @event) => item.Apply(@event));

//         ProjectEvent<ReservationCancelled>((item, @event) => item.Apply(@event));
//     }
// }