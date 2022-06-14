using Core.Extensions;
using Core.Aggregates;
using Innings.Innings.Overs;
using Innings.Innings.Bowlers;
using Innings.Innings.Batsmen;
using Innings.Innings.InitializingInnings;
using Innings.Innings.AssigningBowler;
using Innings.Innings.AssigningBatsman;
using Innings.Innings.UnassigningBatsman;
using Innings.Innings.UnassigningBowler;

namespace Innings.Innings;
public class Innings : Aggregate
{
    public Guid MatchId { get; private set; }
    public Guid BattingTeamId { get; private set; }
    public int InningsNumber { get; private set; }
    public int MaxOvers { get; private set; }
    public int DeliveriesPerOver { get; private set; }
    public bool TieBreaker { get; private set; } = false;
    public int TargetScore { get; private set; }
    public int Wickets { get; private set; }
    public int Runs { get; private set; }
    public IList<InningsOver> Overs { get; private set; } = default!;
    public IList<Batsman> Batsmen { get; private set; } = default!;
    public IList<Bowler> Bowlers { get; private set; } = default!;
    public InningsStatus InningsStatus { get; private set; }

    public static Innings Initialize(
        Guid id,
        Guid matchId,
        Guid battingTeamId,
        int inningsNumber,
        int maxOvers,
        int deliveriesPerOver,
        bool tieBreaker,
        int targetScore,
        IReadOnlyList<Batsman> batsmen
    )
    {
        return new Innings(id, matchId, battingTeamId, inningsNumber, maxOvers, deliveriesPerOver, tieBreaker, targetScore, batsmen);
    }

    public Innings(){}
    private Innings(
        Guid id,
        Guid matchId,
        Guid battingTeamId,
        int inningsNumber,
        int maxOvers,
        int deliveriesPerOver,
        bool tieBreaker,
        int targetScore,
        IReadOnlyList<Batsman> batsmen
    )
    {
        var @event = InningsInitialized.Create(
            id,
            matchId,
            battingTeamId,
            inningsNumber,
            maxOvers,
            deliveriesPerOver,
            tieBreaker,
            targetScore,
            batsmen,
            InningsStatus.Initialized
        );
        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(InningsInitialized @event)
    {
        Id = @event.InningsId;
        MatchId = @event.MatchId;
        BattingTeamId = @event.BattingTeamId;
        InningsNumber = @event.InningsNumber;
        MaxOvers = @event.MaxOvers;
        DeliveriesPerOver = @event.DeliveriesPerOver;
        TieBreaker = @event.TieBreaker;
        TargetScore = @event.TargetScore;
        foreach (var item in @event.Batsmen)
        {
            Batsmen.Add(item);
        }
        Version++;
    }

    public void AssignBowler(Bowler bowler)
    {
        if(InningsStatus != InningsStatus.Initialized)
            throw new InvalidOperationException($"Assigning Bowler for Innings in '{InningsStatus}' status is not allowed.");
        var @event = BowlerEnteredPlay.Create(Id, bowler);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(BowlerEnteredPlay @event)
    {
        Version++;
        var newBowler = @event.Bowler;
        var existingBowler = FindBowlerMatchingWith(newBowler);
        if (existingBowler is null)
        {
            Bowlers.Add(newBowler.EnterPlay(newBowler));
            return;
        }

        Bowlers.Replace(
            existingBowler,
            existingBowler.EnterPlay(newBowler)
        );
    }
    public void AssignBatsman(Batsman batsman)
    {
        if(InningsStatus != InningsStatus.Initialized)
            throw new InvalidOperationException($"Assigning Batsman for Innings in '{InningsStatus}' status is not allowed.");
        
        var @event = BatsmanEnteredPlay.Create(Id, batsman);

        Enqueue(@event);
        Apply(@event);
    }
    public void Apply(BatsmanEnteredPlay @event)
    {
        Version++;
        var newBatsman = @event.Batsman;
        var existingBatsman = FindBatsmanMatchingWith(@event.Batsman);
        if(existingBatsman is null)
        {
            Batsmen.Add(newBatsman.EnterPlay(newBatsman));
            return;
        }

        Batsmen.Replace(
            existingBatsman,
            existingBatsman.EnterPlay(newBatsman)
        );
    }
    public void UnassignBatsman(Batsman batsman, BatsmanState state)
    {
        if(InningsStatus != InningsStatus.Initialized)
            throw new InvalidOperationException($"Unassigning Batsman for Innings in '{InningsStatus}' status is not allowed.");
        var @event = BatsmanLeftPlay.Create(Id, batsman, state);
        Enqueue(@event);
        Apply(@event);
    }
    public void Apply(BatsmanLeftPlay @event)
    {
        Version++;
        var newBatsman = @event.Batsman;
        var existingBatsman = FindBatsmanMatchingWith(@event.Batsman);
        if(existingBatsman is null)
        {
            throw new InvalidOperationException($"Batsman did not exist");
        }
        Batsmen.Replace(
            existingBatsman,
            @event.State == BatsmanState.Dismissed ? existingBatsman.Dismiss(newBatsman) : existingBatsman.Retire(newBatsman)
        );
    }
    public void UnassignBowler(Bowler Bowler)
    {
        if(InningsStatus != InningsStatus.Initialized)
            throw new InvalidOperationException($"Unassigning Bowler for Innings in '{InningsStatus}' status is not allowed.");
        var @event = BowlerLeftPlay.Create(Id, Bowler);
        Enqueue(@event);
        Apply(@event);
    }
    public void Apply(BowlerLeftPlay @event)
    {
        Version++;
        var newBowler = @event.Bowler;
        var existingBowler = FindBowlerMatchingWith(@event.Bowler);
        if(existingBowler is null)
        {
            throw new InvalidOperationException($"Bowler did not exist");
        }
        Bowlers.Replace(
            existingBowler,
            existingBowler.LeavePlay(newBowler)
        );
    }
    private Bowler? FindBowlerMatchingWith(Bowler bowler)
    {
        return Bowlers
            .SingleOrDefault(b => b.MatchesBowler(b));
    }
    private Batsman? FindBatsmanMatchingWith(Batsman batsman)
    {
        return Batsmen
            .SingleOrDefault(b => b.MatchesBatsman(b));
    }
}