using Core.Commands;
using Core.DynamoDbEventStore.Events;
using Core.DynamoDbEventStore.Repository;
using Innings.Innings.Bowlers;
using MediatR;

namespace Innings.Innings.AssigningBowler;

public record AssignBowler(
    Guid InningsId,
    Bowler Bowler
) : ICommand
{
    public static AssignBowler Create(Guid matchId, Bowler bowler)
    {
        if(matchId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(matchId));
        if(bowler == null)
            throw new ArgumentNullException(nameof(bowler));
        return new AssignBowler(matchId, bowler);
    }
}