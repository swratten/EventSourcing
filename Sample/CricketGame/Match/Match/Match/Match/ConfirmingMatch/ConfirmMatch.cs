using Core.Commands;
using Core.DynamoDbEventStore.Events;
using Core.DynamoDbEventStore.Repository;
using MediatR;
namespace Match.Match.ConfirmingMatch;
public record ConfirmMatch(
    Guid MatchId
) : ICommand
{
    public static ConfirmMatch Create(Guid matchId)
    {
        if(matchId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(matchId));
        
        return new ConfirmMatch(matchId);
    }
}

internal class HandleConfirmMatch:
    ICommandHandler<ConfirmMatch>
{
    private readonly IDynamoDBRepository<CricketMatch> repository;
    private readonly IDynamoDBAppendScope scope;

    public HandleConfirmMatch(
        IDynamoDBRepository<CricketMatch> repository,
        IDynamoDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }
    public async Task<Unit> Handle(ConfirmMatch command, CancellationToken cancellationToken)
    {
        await scope.Do((expectedVersion, traceMetadata) =>
            repository.GetAndUpdate(
                command.MatchId,
                match => match.Confirm(),
                expectedVersion,
                traceMetadata,
                cancellationToken
            )
        );
        return Unit.Value;
    }
}