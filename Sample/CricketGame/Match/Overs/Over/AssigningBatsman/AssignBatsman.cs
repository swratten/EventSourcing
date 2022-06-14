using Core.Commands;
using Core.DynamoDbEventStore.Events;
using Core.DynamoDbEventStore.Repository;
using Overs.Over;
using MediatR;

namespace Overs.Over.AssigningBatsman;

public record AssignBatsman(
    Guid OverId,
    Batsman Batsman
) : ICommand
{
    public static AssignBatsman Create(Guid overId, Batsman batsman)
    {
        if(overId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(overId));
        if(batsman == null)
            throw new ArgumentNullException(nameof(batsman));
        return new AssignBatsman(overId, batsman);
    }
}