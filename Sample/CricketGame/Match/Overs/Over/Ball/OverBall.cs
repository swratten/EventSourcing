namespace Overs.Over.Ball;
public class OverBall
{
    public Guid BallId { get; }
    public int BallNumber { get; }
    public Guid StrikingBatsmanId { get; }
    public Guid BowlerId { get; }
    public int Runs { get; }
    public bool IsBoundary { get; }
    public bool IsWicket { get; }
    public Dismissal Dismissal { get; }
}