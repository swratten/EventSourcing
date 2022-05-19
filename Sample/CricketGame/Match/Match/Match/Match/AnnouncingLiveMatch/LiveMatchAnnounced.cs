namespace Match.Match.AnnouncingLiveMatch;
public record LiveMatchAnnounced(
    Guid MatchId,
    DateTime ConfirmedAt
)
{
    public static LiveMatchAnnounced Create(Guid matchId, DateTime announcedAt)
    {
        if (matchId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(matchId));
        if (announcedAt == default)
            throw new ArgumentOutOfRangeException(nameof(announcedAt));

        return new LiveMatchAnnounced(matchId, announcedAt);
    }
}