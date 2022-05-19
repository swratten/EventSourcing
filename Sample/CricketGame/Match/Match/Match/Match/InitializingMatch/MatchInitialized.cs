namespace Match.Match.InitializingMatch;
public record MatchInitialized(
    Guid MatchId,
    Guid TeamOneId,
    Guid TeamTwoId,
    Guid SeasonId,
    Guid VenueId,
    Guid MatchTypeId,
    MatchStatus MatchStatus
)
{
    public static MatchInitialized Create(Guid matchId, Guid teamOneId, Guid teamTwoId, Guid seasonId, Guid venueId, Guid matchType, MatchStatus matchStatus)
    {
        if (matchId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(matchId));
        if (teamOneId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(teamOneId));
        if (teamTwoId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(teamTwoId));
        if (seasonId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(seasonId));
        if (venueId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(venueId));    
        if (matchType == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(matchType));
        if(matchStatus == default)
            throw new ArgumentOutOfRangeException(nameof(matchStatus));
        return new MatchInitialized(matchId, teamOneId, teamTwoId, seasonId, venueId, matchType, matchStatus);
    }
}