using Innings.Innings.Batsmen;

namespace Innings.Innings.AssigningBatsman;

public record BatsmanEnteredPlay(
    Guid InningsId,
    Batsman Batsman
)
{
    public static BatsmanEnteredPlay Create(Guid inningsId, Batsman batsman)
    {
        if(inningsId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(inningsId));
        if(batsman == null)
            throw new ArgumentNullException(nameof(batsman));
        return new BatsmanEnteredPlay(inningsId, batsman);
    }
}