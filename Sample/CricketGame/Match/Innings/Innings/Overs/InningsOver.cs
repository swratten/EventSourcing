namespace Innings.Innings.Overs;
public class InningsOver
{
    public Guid OverId { get; }
    public int OverNumber { get; }
    public bool IsComplete { get; }
    public int Wickets { get; }
    public int Runs { get; }
}