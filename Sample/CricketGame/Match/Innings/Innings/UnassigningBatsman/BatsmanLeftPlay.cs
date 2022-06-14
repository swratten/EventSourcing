using Innings.Innings.Batsmen;

namespace Innings.Innings.UnassigningBatsman;

public record BatsmanLeftPlay(
    Guid InningsId,
    Batsman Batsman,
    BatsmanState State
)
{
    public static BatsmanLeftPlay Create(Guid inningsId, Batsman batsman, BatsmanState state)
    {
        if(inningsId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(inningsId));
        if(batsman == null)
            throw new ArgumentNullException(nameof(batsman));
        return new BatsmanLeftPlay(inningsId, batsman, state);
    }
}