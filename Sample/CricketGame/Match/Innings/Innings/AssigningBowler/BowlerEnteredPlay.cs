using Innings.Innings.Bowlers;

namespace Innings.Innings.AssigningBowler;

public record BowlerEnteredPlay(
    Guid InningsId,
    Bowler Bowler
)
{
    public static BowlerEnteredPlay Create(Guid matchId, Bowler bowler)
    {
        if(matchId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(matchId));
        if(bowler == null)
            throw new ArgumentNullException(nameof(bowler));
        return new BowlerEnteredPlay(matchId, bowler);
    }
}