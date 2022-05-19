namespace Match.Match.ConfirmingMatch;
public record MatchConfirmed(
    Guid MatchId,
    DateTime ConfirmedAt
)
{
    public static MatchConfirmed Create(Guid matchId, DateTime confirmedAt)
    {
        if (matchId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(matchId));
        if (confirmedAt == default)
            throw new ArgumentOutOfRangeException(nameof(confirmedAt));

        return new MatchConfirmed(matchId, confirmedAt);
    }
}