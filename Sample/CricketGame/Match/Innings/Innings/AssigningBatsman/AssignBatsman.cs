using Core.Commands;
using Core.DynamoDbEventStore.Events;
using Core.DynamoDbEventStore.Repository;
using Innings.Innings.Batsmen;
using MediatR;

namespace Innings.Innings.AssigningBatsman;

public record AssignBatsman(
    Guid InningsId,
    Batsman Batsman
) : ICommand
{
    public static AssignBatsman Create(Guid inningsId, Batsman batsman)
    {
        if(inningsId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(inningsId));
        if(batsman == null)
            throw new ArgumentNullException(nameof(batsman));
        return new AssignBatsman(inningsId, batsman);
    }
}

internal class HandleAssignBatsman:
    ICommandHandler<AssignBatsman>
{
    private readonly IDynamoDBRepository<Innings> repository;
    private readonly IDynamoDBAppendScope scope;

    public HandleAssignBatsman(
        IDynamoDBRepository<Innings> repository,
        IDynamoDBAppendScope scope
    )
    {
        this.repository = repository;
        this.scope = scope;
    }
    public async Task<Unit> Handle(AssignBatsman command, CancellationToken cancellationToken)
    {
        await scope.Do((expectedVersion, traceMetadata) =>
            repository.GetAndUpdate(
                command.InningsId,
                innings => innings.AssignBatsman(command.Batsman),
                expectedVersion,
                traceMetadata,
                cancellationToken
            )
        );
        return Unit.Value;
    }
}