namespace Match.Match.AssigningTossWinner;
public record TossWinnerAssigned(
    Guid MatchId,
    Guid TossWinnerId
)
{
    public static TossWinnerAssigned Create(Guid matchId, Guid tossWinnerId)
    {
        if(matchId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(matchId));
        if(tossWinnerId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(tossWinnerId));
        return new TossWinnerAssigned(matchId, tossWinnerId);
    }
}