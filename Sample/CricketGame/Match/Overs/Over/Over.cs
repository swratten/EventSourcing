using Core.Extensions;
using Core.Aggregates;
using Overs.Over.BowlEnd;
using Overs.Over.Ball;
using Overs.Over.InitializingOver;
using Overs.Over.AssigningBatsman;
using Overs.Over.UnassigningBatsman;
using Overs.Over.AssigningBowler;
using Overs.Over.RecordingBall;

namespace Overs.Over;
public class Over : Aggregate
{
    public Guid MatchId { get; private set; }
    public Guid BattingTeamId { get; private set; }
    public Guid InningsId { get; private set; }
    public int InningsNumber { get; private set; }
    public int OverNumber { get; private set; }
    public int DeliveryNumber { get; private set; }
    public int Wickets { get; private set; }
    public int Runs { get; private set; }
    public BowlingEnd BowlingEnd { get; private set; }
    public Batsman BatsmanOne { get; private set; } = default!;
    public Batsman BatsmanTwo { get; private set; } = default!;
    public Guid BowlerId { get; private set; }
    public Guid KeeperId { get; private set; }
    public IList<OverBall> OverBalls { get; private set; } = default!;
    public OverStatus OverStatus { get; private set; }

    public static Over Initialize(
        Guid id, 
        Guid matchId, 
        Guid inningsId,
        Guid battingTeamId,
        Batsman batsmanOne,
        Batsman batsmanTwo,
        int inningsNumber,
        int overNumber,
        Guid bowlerId,
        Guid keeperId,
        BowlingEnd bowlingEnd
    )
    {
        return new Over(id, matchId, inningsId, battingTeamId, batsmanOne, batsmanTwo, inningsNumber, overNumber, bowlerId, keeperId, bowlingEnd);
    }
    public Over(){}
    private Over(
        Guid id, 
        Guid matchId, 
        Guid inningsId,
        Guid battingTeamId,
        Batsman batsmanOne,
        Batsman batsmanTwo,
        int inningsNumber,
        int overNumber,
        Guid bowlerId,
        Guid keeperId,
        BowlingEnd bowlingEnd
    )
    {
        var @event = OverInitialized.Create(
            id,
            matchId,
            inningsId,
            battingTeamId,
            batsmanOne,
            batsmanTwo,
            inningsNumber,
            overNumber,
            bowlerId,
            keeperId,
            bowlingEnd,
            OverStatus.Initialized
        );
        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(OverInitialized @event)
    {
        Id = @event.OverId;
        MatchId = @event.MatchId;
        InningsId = @event.InningsId;
        BattingTeamId = @event.BattingTeamId;
        BatsmanOne = @event.BatsmanOne;
        BatsmanTwo = @event.BatsmanTwo;
        InningsNumber = @event.InningsNumber;
        OverNumber = @event.OverNumber;
        BowlerId = @event.BowlerId;
        KeeperId = @event.KeeperId;
        BowlingEnd = @event.BowlingEnd;
        OverStatus = @event.OverStatus;
        Version++;
    }

    public void AssignBatsman(Batsman batsman)
    {
        if(OverStatus != OverStatus.Initialized)
            throw new InvalidOperationException($"Assigning Batsman for Over in '{OverStatus}' status is not allowed.");
        var @event = BatsmanEnteredPlay.Create(Id, batsman);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(BatsmanEnteredPlay @event)
    {
        Version++;
        var newBatsman = @event.Batsman;
        if(BatsmanOne.InPlay == false)
        {
            BatsmanOne = newBatsman;
        }
        else if(BatsmanTwo.InPlay == false)
        {
            BatsmanTwo = newBatsman;
        }
        else
        {
            throw new InvalidOperationException($"No Batsman to replace");
        }
    }

    public void UnassignBatsman(Batsman batsman)
    {
        if(OverStatus != OverStatus.Initialized)
            throw new InvalidOperationException($"Assigning Batsman for Over in '{OverStatus}' status is not allowed.");
        var @event = BatsmanLeftPlay.Create(Id, batsman);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(BatsmanLeftPlay @event)
    {
        Version++;
        var newBatsman = @event.Batsman;
        if(BatsmanOne.BatsmanId == @event.Batsman.BatsmanId && BatsmanOne.InPlay == true)
        {
            BatsmanOne = newBatsman;
        }
        else if(BatsmanTwo.BatsmanId == @event.Batsman.BatsmanId && BatsmanTwo.InPlay == true)
        {
            BatsmanTwo = newBatsman;
        }
        else
        {
            throw new InvalidOperationException($"Batsman was not in play");
        }
    }

    public void AssignBowler(Guid bowlerId)
    {
        if(OverStatus != OverStatus.Initialized)
            throw new InvalidOperationException($"Assigning Bowler for Over in '{OverStatus}' status is not allowed.");
        var @event = BowlerEnteredPlay.Create(Id, bowlerId);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(BowlerEnteredPlay @event)
    {
        Version++;
        BowlerId = @event.BowlerId;
    }

    public void RecordBall(OverBall overBall)
    {
        if(OverStatus != OverStatus.Initialized)
            throw new InvalidOperationException($"Recording ball for Over in '{OverStatus}' status is not allowed.");

        if(OverBalls.FirstOrDefault( ob => ob.BallId == overBall.BallId ) != null)
        {
            throw new InvalidOperationException($"Ball with ID '{overBall.BallId}' already exists.");
        }

        var @event = BallRecorded.Create(Id, overBall);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(BallRecorded @event)
    {
        Version++;
        OverBalls.Add(@event.Ball);
    }
}