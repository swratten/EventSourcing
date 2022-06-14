using Core.Commands;
using Core.DynamoDbEventStore.Events;
using Core.DynamoDbEventStore.Repository;
using Overs.Over;
using MediatR;
using Overs.Over.Ball;

namespace Overs.Over.UnassigningBatsman;

public record UnassignBatsman(
    Guid OverId,
    Batsman Batsman,
    Dismissal Dismissal
) : ICommand
{
    public static UnassignBatsman Create(Guid overId, Batsman batsman, Dismissal dismissal)
    {
        if(overId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(overId));
        if(batsman == null)
            throw new ArgumentNullException(nameof(batsman));
        if(dismissal == default)
            throw new ArgumentOutOfRangeException(nameof(dismissal));
        return new UnassignBatsman(overId, batsman, dismissal);
    }
}