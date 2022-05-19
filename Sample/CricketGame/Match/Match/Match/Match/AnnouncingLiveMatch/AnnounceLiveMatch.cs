using Core.Commands;
using Core.DynamoDbEventStore.Events;
using Core.DynamoDbEventStore.Repository;
using MediatR;
namespace Match.Match.AnnouncingLiveMatch;
public record AnnounceLiveMatch(
    Guid MatchId
) : ICommand
{
    public static AnnounceLiveMatch Create(Guid matchId)
    {
        if(matchId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(matchId));
        
        return new AnnounceLiveMatch(matchId);
    }
}
internal class HandleAnnounceLiveMatch:
    ICommandHandler<AnnounceLiveMatch>
{
    private readonly IDynamoDBRepository<CricketMatch> repository;
    private readonly IDynamoDBAppendScope scope;

    public HandleAnnounceLiveMatch(
        IDynamoDBRepository<CricketMatch> repository,
        IDynamoDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }
    public async Task<Unit> Handle(AnnounceLiveMatch command, CancellationToken cancellationToken)
    {
        await scope.Do((expectedVersion, traceMetadata) =>
            repository.GetAndUpdate(
                command.MatchId,
                match => match.GoLive(),
                expectedVersion,
                traceMetadata,
                cancellationToken
            )
        );
        return Unit.Value;
    }
}