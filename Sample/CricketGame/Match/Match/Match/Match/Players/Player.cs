namespace Match.Match.Players;

public class Player
{
    public Guid PlayerId { get; }
    public Guid TeamId { get; }
    public bool IsCaptain { get; }
    public bool IsKeeper { get; }
    public int BatOrder { get; }
    private Player(Guid playerId, Guid teamId, bool isCaptain, bool isKeeper, int batOrder)
    {
        PlayerId = playerId;
        TeamId = teamId;
        IsCaptain = isCaptain;
        IsKeeper = isKeeper;
        BatOrder = batOrder;
    }
    public static Player Create(Guid? playerId, Guid? teamId, bool isCaptain = false, bool isKeeper = false, int batOrder = int.MinValue)
    {
        if (playerId == null || playerId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(playerId));
        if (teamId == null || teamId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(teamId));
        if(batOrder == int.MinValue)
            throw new ArgumentOutOfRangeException(nameof(batOrder));
        return new Player(playerId.Value, teamId.Value, isCaptain, isKeeper, batOrder);
    }
    public Player MergeWith(Player player)
    {
        if(!MatchesPlayer(player))
            throw new ArgumentException("Player does not match");
        return Create(PlayerId, player.TeamId, player.IsCaptain, player.IsKeeper, player.BatOrder);
    }
    public bool MatchesPlayer(Player player)
    {
        return PlayerId == player.PlayerId;
    }
}