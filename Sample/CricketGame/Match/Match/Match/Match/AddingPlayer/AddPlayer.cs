using Core.Commands;
using Core.DynamoDbEventStore.Events;
using Core.DynamoDbEventStore.Repository;
using Match.Match.Players;
using MediatR;

namespace Match.Match.AddingPlayer;

public record AddPlayer(
    Guid MatchId,
    Player Player
) : ICommand
{
    public static AddPlayer Create(Guid matchId, Player player)
    {
        if(matchId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(matchId));
        return new AddPlayer(matchId, player);
    }
}

internal class HandlePlayerAdded:
    ICommandHandler<AddPlayer>
{
    private readonly IDynamoDBRepository<CricketMatch> repository;
    private readonly IDynamoDBAppendScope scope;

    public HandlePlayerAdded(
        IDynamoDBRepository<CricketMatch> repository,
        IDynamoDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }
    public async Task<Unit> Handle(AddPlayer command, CancellationToken cancellationToken)
    {
        var (matchId, player) = command;

        await scope.Do((expectedVersion, traceMetadata) =>
            repository.GetAndUpdate(
                matchId,
                match => match.AddPlayer(player),
                expectedVersion,
                traceMetadata,
                cancellationToken
            )
        );
        return Unit.Value;
    }
}