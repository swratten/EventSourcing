namespace Innings.Innings.Batsmen;
public class Batsman
{
    Guid PlayerId { get; }
    BatsmanState BatsmanState { get; }
    int BatOrder { get; }
    int Overs { get; }
    int Runs { get; }
    int Fours { get; }
    int Sixes { get; }
    int BallsFaced { get; }

    private Batsman(Guid playerId, int batOrder, int overs, int runs, int fours, int sixes, int ballsFaced, BatsmanState batsmanState)
    {
        PlayerId = playerId;
        BatOrder = batOrder;
        Overs = overs;
        Runs = runs;
        Fours = fours;
        Sixes = sixes;
        BallsFaced = ballsFaced;
        BatsmanState = batsmanState;
    }
    public Batsman EnterPlay(Batsman batsman)
    {
        if(!MatchesBatsman(batsman))
            throw new ArgumentException("Batsman does not match");
        if(BatsmanState == BatsmanState.Dismissed)
            throw new InvalidOperationException($"Player in '{BatsmanState}' is not allowed to Enter Play.");
        return Create(batsman.PlayerId, batsman.BatOrder, batsman.Overs, batsman.Runs, batsman.Fours, batsman.Sixes, batsman.BallsFaced, BatsmanState.InPlay);
    }
    public Batsman Dismiss(Batsman batsman)
    {
        if(!MatchesBatsman(batsman))
            throw new ArgumentException("Batsman does not match");
        if(BatsmanState == BatsmanState.Initialized)
            throw new InvalidOperationException($"Player in '{BatsmanState}' cannot be dismissed before entering Play");
        return Create(batsman.PlayerId, batsman.BatOrder, batsman.Overs, batsman.Runs, batsman.Fours, batsman.Sixes, batsman.BallsFaced, BatsmanState.Dismissed);
    }
    public Batsman Retire(Batsman batsman)
    {
        if(!MatchesBatsman(batsman))
            throw new ArgumentException("Batsman does not match");
        if(BatsmanState == BatsmanState.Initialized)
            throw new InvalidOperationException($"Player in '{BatsmanState}' cannot be retired before entering Play");
        return Create(batsman.PlayerId, batsman.BatOrder, batsman.Overs, batsman.Runs, batsman.Fours, batsman.Sixes, batsman.BallsFaced, BatsmanState.Dismissed);
    }
    public Batsman MergeWith(Batsman batsman)
    {
        if(!MatchesBatsman(batsman))
            throw new ArgumentException("Batsman does not match");
        return Create(batsman.PlayerId, batsman.BatOrder, batsman.Overs, batsman.Runs, batsman.Fours, batsman.Sixes, batsman.BallsFaced, batsman.BatsmanState);
    }
    public bool MatchesBatsman(Batsman batsman)
    {
        return PlayerId == batsman.PlayerId;
    }
    public static Batsman Create(Guid playerId, int batOrder, int overs, int runs, int fours, int sixes, int ballsFaced, BatsmanState batsmanState)
    {
        return new Batsman(playerId, batOrder, overs, runs, fours, sixes, ballsFaced, batsmanState);
    }
}