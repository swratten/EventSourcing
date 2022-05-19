using Match.Match.Players;

namespace Match.Match.AddingPlayer;

public record PlayerAdded(
    Guid MatchId,
    Player Player
)
{
    public static PlayerAdded Create(Guid matchId, Player player)
    {
        if(matchId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(matchId));
        return new PlayerAdded(matchId, player);
    }
}