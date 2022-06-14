using Overs.Over;

namespace Overs.Over.UnassigningBatsman;

public record BatsmanLeftPlay(
    Guid OverId,
    Batsman Batsman
)
{
    public static BatsmanLeftPlay Create(Guid overId, Batsman batsman)
    {
        if(overId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(overId));
        if(batsman == null)
            throw new ArgumentNullException(nameof(batsman));
        return new BatsmanLeftPlay(overId, batsman);
    }
}