using Core.Events;
using Match.Match.Players;

namespace Match.Match.AnnouncingTossWinner;
public record TossWinnerAnnounced (
    Guid MatchId,
    Guid TossWinnerId,
    IReadOnlyList<Player> Players,
    DateTime AnnouncedAt
) : IExternalEvent
{
    public static TossWinnerAnnounced Create(
        Guid matchId,
        Guid tossWinnerId,
        IReadOnlyList<Player> players,
        DateTime announcedAt
    )
    {
        return new TossWinnerAnnounced(matchId, tossWinnerId, players, announcedAt);
    }
}