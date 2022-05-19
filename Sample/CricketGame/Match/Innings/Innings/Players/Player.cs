namespace Innings.Innings.Players;

public class Player
{
    public Guid PlayerId { get; }
    public Guid TeamId { get; }
    public bool IsCaptain { get; }
    public bool IsKeeper { get; }
    public int BatOrder { get; }
}