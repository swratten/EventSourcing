using Innings.Innings.Bowlers;

namespace Innings.Innings.UnassigningBowler;

public record BowlerLeftPlay(
    Guid InningsId,
    Bowler Bowler
)
{
    public static BowlerLeftPlay Create(Guid inningsId, Bowler bowler)
    {
        if(inningsId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(inningsId));
        if(bowler == null)
            throw new ArgumentNullException(nameof(bowler));
        return new BowlerLeftPlay(inningsId, bowler);
    }
}