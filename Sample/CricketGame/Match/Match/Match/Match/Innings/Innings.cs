namespace Match.Match.Innings;
public class Innings
{
    public Guid InningsId { get; }
    public Guid BattingTeamId { get; }
    public int InningsNumber { get; }
    public int Wickets { get; }
    public int Runs { get; }
    public int Overs { get; }
    // public Guid OverId { get; }
    public bool IsComplete { get; }
}