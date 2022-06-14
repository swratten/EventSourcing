using Core.Commands;
using Core.DynamoDbEventStore.Events;
using Core.DynamoDbEventStore.Repository;
using MediatR;

namespace Overs.Over.UnassigningBowler;

public record UnassignBowler(
    Guid OverId,
    Guid BowlerId
) : ICommand
{
    public static UnassignBowler Create(Guid overId, Guid bowlerId)
    {
        if(overId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(overId));
        if(bowlerId == null || bowlerId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(bowlerId));
        return new UnassignBowler(overId, bowlerId);
    }
}