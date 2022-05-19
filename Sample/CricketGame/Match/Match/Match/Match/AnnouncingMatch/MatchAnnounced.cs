using Core.Events;
using Match.Match.Players;

namespace Match.Match.AnnoucingMatch;
public record MatchAnnounced (
    Guid MatchId,
    Guid TeamOneId,
    Guid TeamTwoId,
    Guid SeasonId,
    Guid VenueId,
    Guid MatchTypeId,
    IReadOnlyList<Player> Players,
    DateTime AnnouncedAt
) : IExternalEvent
{
    public static MatchAnnounced Create(
        Guid matchId,
        Guid teamOneId,
        Guid teamTwoId,
        Guid seasonId,
        Guid venueId,
        Guid matchTypeId,
        IReadOnlyList<Player> players,
        DateTime announcedAt
    )
    {
        return new MatchAnnounced(matchId, teamOneId, teamTwoId, seasonId, venueId, matchTypeId, players, announcedAt);
    }
}