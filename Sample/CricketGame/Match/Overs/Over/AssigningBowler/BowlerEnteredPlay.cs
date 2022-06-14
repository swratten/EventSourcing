namespace Overs.Over.AssigningBowler;

public record BowlerEnteredPlay(
    Guid OverId,
    Guid BowlerId
)
{
    public static BowlerEnteredPlay Create(Guid overId, Guid bowlerId)
    {
        if(overId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(overId));
        if(bowlerId == null || bowlerId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(bowlerId));
        return new BowlerEnteredPlay(overId, bowlerId);
    }
}