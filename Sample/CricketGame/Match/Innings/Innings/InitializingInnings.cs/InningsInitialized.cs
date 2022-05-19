using Innings.Innings.Batsmen;

namespace Innings.Innings.InitializingInnings;

public record InningsInitialized (
    Guid InningsId,
    Guid MatchId,
    Guid BattingTeamId,
    int InningsNumber,
    int MaxOvers,
    int DeliveriesPerOver,
    bool TieBreaker,
    int TargetScore,
    IReadOnlyList<Batsman> Batsmen,
    InningsStatus InningsStatus
)
{
    public static InningsInitialized Create(
        Guid inningsId,
        Guid matchId,
        Guid battingTeamId,
        int inningsNumber,
        int maxOvers,
        int deliveriesPerOver,
        bool tieBreaker,
        int targetScore,
        IReadOnlyList<Batsman> batsmen,
        InningsStatus inningsStatus
    )
    {
        if(inningsId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(inningsId));
        if(inningsId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(inningsId));
        if(inningsId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(inningsId));
        if(batsmen == null)
            throw new ArgumentNullException(nameof(batsmen));
        if(inningsStatus == default)
            throw new ArgumentOutOfRangeException(nameof(inningsStatus));
        
        return new InningsInitialized(inningsId, matchId, battingTeamId, inningsNumber, maxOvers, deliveriesPerOver, tieBreaker, targetScore, batsmen, inningsStatus);
    }
}