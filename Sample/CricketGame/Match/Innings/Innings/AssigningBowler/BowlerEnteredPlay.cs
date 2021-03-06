using Innings.Innings.Bowlers;

namespace Innings.Innings.AssigningBowler;

public record BowlerEnteredPlay(
    Guid InningsId,
    Bowler Bowler
)
{
    public static BowlerEnteredPlay Create(Guid inningsId, Bowler bowler)
    {
        if(inningsId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(inningsId));
        if(bowler == null)
            throw new ArgumentNullException(nameof(bowler));
        return new BowlerEnteredPlay(inningsId, bowler);
    }
}