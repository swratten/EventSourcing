using Core.Commands;
using Core.DynamoDbEventStore.Events;
using Core.DynamoDbEventStore.Repository;
using MediatR;
namespace Match.Match.AssigningTossWinner;
public record AssignTossWinner(
    Guid MatchId,
    Guid TossWinnerId
) : ICommand
{
    public static AssignTossWinner Create(Guid matchId, Guid tossWinnerId)
    {
        if(matchId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(matchId));
        if(tossWinnerId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(tossWinnerId));
        return new AssignTossWinner(matchId, tossWinnerId);
    }
}
internal class HandleAssignTossWinner:
    ICommandHandler<AssignTossWinner>
{
    private readonly IDynamoDBRepository<CricketMatch> repository;
    private readonly IDynamoDBAppendScope scope;

    public HandleAssignTossWinner(
        IDynamoDBRepository<CricketMatch> repository,
        IDynamoDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }
    public async Task<Unit> Handle(AssignTossWinner command, CancellationToken cancellationToken)
    {
        var (matchId, tossWinnerId) = command;

        await scope.Do((expectedVersion, traceMetadata) =>
            repository.GetAndUpdate(
                matchId,
                match => match.AssignTossWinner(tossWinnerId),
                expectedVersion,
                traceMetadata,
                cancellationToken
            )
        );
        return Unit.Value;
    }
}