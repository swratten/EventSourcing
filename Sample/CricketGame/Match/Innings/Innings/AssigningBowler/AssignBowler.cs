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
    public static AssignBowler Create(Guid inningsId, Bowler bowler)
    {
        if(inningsId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(inningsId));
        if(bowler == null)
            throw new ArgumentNullException(nameof(bowler));
        return new AssignBowler(inningsId, bowler);
    }
}

internal class HandleAssignBowler:
    ICommandHandler<AssignBowler>
{
    private readonly IDynamoDBRepository<Innings> repository;
    private readonly IDynamoDBAppendScope scope;

    public HandleAssignBowler(
        IDynamoDBRepository<Innings> repository,
        IDynamoDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }
    public async Task<Unit> Handle(AssignBowler command, CancellationToken cancellationToken)
    {
        await scope.Do((expectedVersion, traceMetadata) =>
            repository.GetAndUpdate(
                command.InningsId,
                innings => innings.AssignBowler(command.Bowler),
                expectedVersion,
                traceMetadata,
                cancellationToken
            )
        );
        return Unit.Value;
    }
}