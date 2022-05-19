namespace Match.Match.Overs;

public class Over
{
    public Guid OverId { get; }
    public int OverNumber { get; }
    public Guid InningsId { get; }
    public bool IsComplete { get; }
    public int Wickets { get; }
    public int Runs { get; }
}