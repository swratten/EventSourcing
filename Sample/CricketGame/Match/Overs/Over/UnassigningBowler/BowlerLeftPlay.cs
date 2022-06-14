namespace Overs.Over.UnassigningBowler;

public record BowlerLeftPlay(
    Guid OverId,
    Guid BowlerId
)
{
    public static BowlerLeftPlay Create(Guid overId, Guid bowlerId)
    {
        if(overId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(overId));
        if(bowlerId == null || bowlerId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(bowlerId));
        return new BowlerLeftPlay(overId, bowlerId);
    }
}