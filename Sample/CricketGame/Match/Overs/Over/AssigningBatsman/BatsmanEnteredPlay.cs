using Overs.Over;

namespace Overs.Over.AssigningBatsman;

public record BatsmanEnteredPlay(
    Guid OverId,
    Batsman Batsman
)
{
    public static BatsmanEnteredPlay Create(Guid overId, Batsman batsman)
    {
        if(overId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(overId));
        if(batsman == null)
            throw new ArgumentNullException(nameof(batsman));
        return new BatsmanEnteredPlay(overId, batsman);
    }
}