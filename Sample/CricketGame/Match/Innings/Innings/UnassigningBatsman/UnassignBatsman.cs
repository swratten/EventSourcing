using Core.Commands;
using Core.DynamoDbEventStore.Events;
using Core.DynamoDbEventStore.Repository;
using Innings.Innings.Batsmen;
using MediatR;

namespace Innings.Innings.UnassigningBatsman;

public record UnassignBatsman(
    Guid InningsId,
    Batsman Batsman,
    BatsmanState state
) : ICommand
{
    public static UnassignBatsman Create(Guid inningsId, Batsman batsman, BatsmanState state)
    {
        if(inningsId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(inningsId));
        if(batsman == null)
            throw new ArgumentNullException(nameof(batsman));
        if(state == default)
            throw new ArgumentOutOfRangeException(nameof(state));
        return new UnassignBatsman(inningsId, batsman, state);
    }
}

internal class HandleUnassignBatsman:
    ICommandHandler<UnassignBatsman>
{
    private readonly IDynamoDBRepository<Innings> repository;
    private readonly IDynamoDBAppendScope scope;

    public HandleUnassignBatsman(
        IDynamoDBRepository<Innings> repository,
        IDynamoDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }
    public async Task<Unit> Handle(UnassignBatsman command, CancellationToken cancellationToken)
    {
        await scope.Do((expectedVersion, traceMetadata) =>
            repository.GetAndUpdate(
                command.InningsId,
                innings => innings.UnassignBatsman(command.Batsman, command.state),
                expectedVersion,
                traceMetadata,
                cancellationToken
            )
        );
        return Unit.Value;
    }
}