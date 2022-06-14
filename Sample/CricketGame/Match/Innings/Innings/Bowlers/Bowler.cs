namespace Innings.Innings.Bowlers;
public class Bowler
{
    Guid PlayerId { get; }
    bool InPlay { get; }
    int Overs { get; }
    int Wickets { get; }
    int Maidens { get; }
    int Runs { get; }
    private Bowler(Guid playerId, bool inPlay, int overs, int wickets, int maidens, int runs)
    {
        PlayerId = playerId;
        InPlay = inPlay;
        Overs = overs;
        Wickets = wickets;
        Maidens = maidens;
        Runs = runs;
    }
    public static Bowler Create(Guid playerId, bool inPlay, int overs, int wickets, int maidens, int runs)
    {
        return new Bowler(playerId, inPlay, overs, wickets, maidens, runs);
    }
    public Bowler EnterPlay(Bowler bowler)
    {
        if(!MatchesBowler(bowler))
            throw new ArgumentException("Bowler does not match");
        return Create(bowler.PlayerId, true, bowler.Overs, bowler.Wickets, bowler.Maidens, bowler.Runs);
    }
    public Bowler LeavePlay(Bowler bowler)
    {
        if(!MatchesBowler(bowler))
            throw new ArgumentException("Bowler does not match");
        return Create(bowler.PlayerId, false, bowler.Overs, bowler.Wickets, bowler.Maidens, bowler.Runs);
    }
    public Bowler MergeWith(Bowler bowler)
    {
        if(!MatchesBowler(bowler))
            throw new ArgumentException("Bowler does not match");
        return Create(PlayerId, bowler.InPlay, bowler.Overs, bowler.Wickets, bowler.Maidens, bowler.Runs);
    }
    public bool MatchesBowler(Bowler bowler)
    {
        return PlayerId == bowler.PlayerId;
    }
}