using Core.Commands;
using Core.DynamoDbEventStore.Events;
using Core.DynamoDbEventStore.Repository;
using Innings.Innings.Batsmen;
using Innings.Innings.Players;
using MediatR;

namespace Innings.Innings.InitializingInnings;

public record InitializeInnings (
    Guid InningsId,
    Guid MatchId,
    Guid BattingTeamId,
    int InningsNumber,
    int MaxOvers,
    int DeliveriesPerOver,
    bool TieBreaker,
    int TargetScore,
    IReadOnlyList<Batsman> Batsmen
) : ICommand
{
    public static InitializeInnings Create(
        Guid? inningsId,
        Guid? matchId,
        Guid? battingTeamId,
        int? inningsNumber,
        int? maxOvers,
        int? deliveriesPerOver,
        bool? tieBreaker,
        int? targetScore,
        IReadOnlyList<Batsman>? batsmen
    )
    {
        if(!inningsId.HasValue)
            throw new ArgumentNullException(nameof(inningsId));
        if(!matchId.HasValue)
            throw new ArgumentNullException(nameof(matchId));
        if(!battingTeamId.HasValue)
            throw new ArgumentNullException(nameof(battingTeamId));
        if(!inningsNumber.HasValue)
            throw new ArgumentNullException(nameof(inningsNumber));
        if(!maxOvers.HasValue)
            throw new ArgumentNullException(nameof(maxOvers));
        if(!deliveriesPerOver.HasValue)
            throw new ArgumentNullException(nameof(deliveriesPerOver));
        if(!tieBreaker.HasValue)
            throw new ArgumentNullException(nameof(tieBreaker));
        if(!targetScore.HasValue)
            throw new ArgumentNullException(nameof(targetScore));
        if(batsmen == null)
            throw new ArgumentNullException(nameof(batsmen));
        
        return new InitializeInnings(inningsId.Value, matchId.Value, battingTeamId.Value, inningsNumber.Value, maxOvers.Value, deliveriesPerOver.Value, tieBreaker.Value, targetScore.Value, batsmen);
    }
}
internal class HandleInitializeInnings:
    ICommandHandler<InitializeInnings>
{
    private readonly IDynamoDBRepository<Innings> repository;
    private readonly IDynamoDBAppendScope scope;

    public HandleInitializeInnings(
        IDynamoDBRepository<Innings> repository,
        IDynamoDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }
    public async Task<Unit> Handle(InitializeInnings command, CancellationToken cancellationToken)
    {
        var (inningsId, matchId, battingTeamId, inningsNumber, maxOvers, deliveriesPerOver, tieBreaker, targetScore, batsmen) = command;

        await scope.Do((_, eventMetadata) =>
            repository.Add(
                Innings.Initialize( inningsId, matchId, battingTeamId, inningsNumber, maxOvers, deliveriesPerOver, tieBreaker, targetScore, batsmen),
                eventMetadata,
                cancellationToken
            )
        );
        return Unit.Value;
    }
}