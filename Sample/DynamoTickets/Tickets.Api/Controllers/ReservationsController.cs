using Core.Commands;
using Core.Ids;
using Core.Queries;
using Microsoft.AspNetCore.Mvc;
using Tickets.Api.Requests;
using Tickets.Api.Responses;
using Tickets.Reservations;
using Tickets.Reservations.CancellingReservation;
using Tickets.Reservations.ChangingReservationSeat;
using Tickets.Reservations.ConfirmingReservation;
using Tickets.Reservations.CreatingTentativeReservation;
using Tickets.Reservations.GettingReservationAtVersion;
using Tickets.Reservations.GettingReservationById;
using Tickets.Reservations.GettingReservationHistory;
using Tickets.Reservations.GettingReservations;

namespace Tickets.Api.Controllers;

[Route("api/[controller]")]
public class ReservationsController: Controller
{
    private readonly ICommandBus commandBus;
    private readonly IQueryBus queryBus;

    public ReservationsController(
        ICommandBus commandBus,
        IQueryBus queryBus,
        IIdGenerator idGenerator)
    {
        this.commandBus = commandBus;
        this.queryBus = queryBus;
    }

    [HttpGet("{id}")]
    public Task<ReservationDetails> Get(Guid id)
    {
        return queryBus.Send<GetReservationById, ReservationDetails>(new GetReservationById(id));
    }

    [HttpGet]
    public async Task<IReadOnlyList<ReservationShortInfo>> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
    {
        var pagedList = await queryBus.Send<GetReservations, IReadOnlyList<ReservationShortInfo>>(GetReservations.Create(pageNumber, pageSize));

        return pagedList;
    }


    [HttpGet("{id}/history")]
    public async Task<IReadOnlyList<ReservationHistory>> GetHistory(Guid id)
    {
        var pagedList = await queryBus.Send<GetReservationHistory, IReadOnlyList<ReservationHistory>>(GetReservationHistory.Create(id));

        return pagedList;
    }

    [HttpGet("{id}/versions")]
    public Task<ReservationDetails> GetVersion(Guid id, [FromQuery] GetReservationDetailsAtVersion request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return queryBus.Send<GetReservationAtVersion, ReservationDetails>(GetReservationAtVersion.Create(id, request.Version));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTentative([FromBody] CreateTentativeReservationRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var reservationId = Guid.NewGuid();

        var command = CreateTentativeReservation.Create(
            reservationId,
            request.SeatId
        );

        await commandBus.Send(command);

        return Created("api/Reservations", reservationId);
    }


    [HttpPost("{id}/seat")]
    public async Task<IActionResult> ChangeSeat(Guid id, [FromBody] ChangeSeatRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var command = ChangeReservationSeat.Create(
            id,
            request.SeatId
        );

        await commandBus.Send(command);

        return Ok();
    }

    [HttpPut("{id}/confirmation")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        var command = ConfirmReservation.Create(
            id
        );

        await commandBus.Send(command);

        return Ok();
    }



    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var command = CancelReservation.Create(
            id
        );

        await commandBus.Send(command);

        return Ok();
    }
}