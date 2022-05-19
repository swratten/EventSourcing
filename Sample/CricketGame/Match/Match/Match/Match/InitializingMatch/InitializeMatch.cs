using Core.Commands;
using Core.DynamoDbEventStore.Events;
using Core.DynamoDbEventStore.Repository;
using MediatR;

namespace Match.Match.InitializingMatch;

public record InitializeMatch(
    Guid MatchId,
    Guid TeamOneId,
    Guid TeamTwoId,
    Guid SeasonId,
    Guid VenueId,
    Guid MatchTypeId
) : ICommand
{
    public static InitializeMatch Create(Guid? matchId, Guid? teamOneId, Guid? teamTwoId, Guid? seasonId, Guid? venueId, Guid? matchType)
    {
        if (matchId == null || matchId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(matchId));
        if (teamOneId == null || teamOneId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(teamOneId));
        if (teamTwoId == null || teamTwoId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(teamTwoId));
        if (seasonId == null || seasonId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(seasonId));
        if (venueId == null || venueId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(venueId));    
        if (matchType == null || matchType == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(matchType));
        return new InitializeMatch(matchId.Value, teamOneId.Value, teamTwoId.Value, seasonId.Value, venueId.Value, matchType.Value);
    }
}
internal class HandleInitializeMatch:
    ICommandHandler<InitializeMatch>
{
    private readonly IDynamoDBRepository<CricketMatch> repository;
    private readonly IDynamoDBAppendScope scope;

    public HandleInitializeMatch(
        IDynamoDBRepository<CricketMatch> repository,
        IDynamoDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }
    public async Task<Unit> Handle(InitializeMatch command, CancellationToken cancellationToken)
    {
        var (matchId, teamOneId, teamTwoId, seasonId, venueId, matchTypeId) = command;

        await scope.Do((_, eventMetadata) =>
            repository.Add(
                CricketMatch.Initialize(matchId, teamOneId, teamTwoId, seasonId, venueId, matchTypeId),
                eventMetadata,
                cancellationToken
            )
        );
        return Unit.Value;
    }
}