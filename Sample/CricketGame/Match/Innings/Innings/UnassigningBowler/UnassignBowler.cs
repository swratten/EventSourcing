using Core.Commands;
using Core.DynamoDbEventStore.Events;
using Core.DynamoDbEventStore.Repository;
using Innings.Innings.Bowlers;
using MediatR;

namespace Innings.Innings.UnassigningBowler;

public record UnassignBowler(
    Guid InningsId,
    Bowler Bowler
) : ICommand
{
    public static UnassignBowler Create(Guid inningsId, Bowler bowler)
    {
        if(inningsId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(inningsId));
        if(bowler == null)
            throw new ArgumentNullException(nameof(bowler));
        return new UnassignBowler(inningsId, bowler);
    }
}

internal class HandleUnassignBowler:
    ICommandHandler<UnassignBowler>
{
    private readonly IDynamoDBRepository<Innings> repository;
    private readonly IDynamoDBAppendScope scope;

    public HandleUnassignBowler(
        IDynamoDBRepository<Innings> repository,
        IDynamoDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }
    public async Task<Unit> Handle(UnassignBowler command, CancellationToken cancellationToken)
    {
        await scope.Do((expectedVersion, traceMetadata) =>
            repository.GetAndUpdate(
                command.InningsId,
                innings => innings.UnassignBowler(command.Bowler),
                expectedVersion,
                traceMetadata,
                cancellationToken
            )
        );
        return Unit.Value;
    }
}