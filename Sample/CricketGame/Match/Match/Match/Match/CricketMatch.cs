using Core.Aggregates;
using Match.Match.Players;
using Match.Match.ConfirmingMatch;
using Match.Match.InitializingMatch;
using Match.Match.AddingPlayer;
using Match.Match.AssigningTossWinner;
using Match.Match.AnnouncingLiveMatch;
using Core.Extensions;

namespace Match.Match;
public class CricketMatch : Aggregate
{
    public Guid TeamOneId { get; private set; }
    public Guid TeamTwoId { get; private set; }
    public Guid SeasonId { get; private set; }
    public Guid VenueId { get; private set; }
    public Guid MatchTypeId { get; private set; }
    public Guid TossWinnerId { get; private set; }
    public MatchStatus MatchStatus { get; private set; }
    public IList<Player> Players { get; private set; } = default!;
    public IList<Innings.Innings> Innings { get; private set; } = default!;
    public static CricketMatch Initialize(
        Guid id,
        Guid teamOneId,
        Guid teamTwoId,
        Guid seasonId,
        Guid venueId,
        Guid matchTypeId
    )
    {
        return new CricketMatch(id, teamOneId, teamTwoId, seasonId, venueId, matchTypeId);
    }
    
    public CricketMatch(){}
    private CricketMatch(
        Guid id,
        Guid teamOneId,
        Guid teamTwoId,
        Guid seasonId,
        Guid venueId,
        Guid matchTypeId
    )
    {
        var @event = MatchInitialized.Create(
            id,
            teamOneId,
            teamTwoId,
            seasonId,
            venueId,
            matchTypeId,
            MatchStatus.Initialized
        );

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(MatchInitialized @event)
    {
        Id = @event.MatchId;
        TeamOneId = @event.TeamOneId;
        TeamTwoId = @event.TeamTwoId;
        SeasonId = @event.SeasonId;
        VenueId = @event.VenueId;
        MatchTypeId = @event.MatchTypeId;
        MatchStatus = @event.MatchStatus;
        Version++;
    }

    public void AddPlayer(Player player)
    {
        if(MatchStatus != MatchStatus.Initialized)
            throw new InvalidOperationException($"Adding Player for the match in '{MatchStatus}' status is not allowed.");
        var @event = PlayerAdded.Create(Id, player);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(PlayerAdded @event)
    {
        Version++;

        var newPlayer = @event.Player;

        var existingPlayerItem = FindPlayerItemMatchingWith(newPlayer);

        if (existingPlayerItem is null)
        {
            Players.Add(newPlayer);
            return;
        }

        Players.Replace(
            existingPlayerItem,
            existingPlayerItem.MergeWith(newPlayer)
        );
    }
    
    public void Confirm()
    {
        if(MatchStatus != MatchStatus.Initialized)
            throw new InvalidOperationException($"Confirming Match in '{MatchStatus}' status is not allowed.");
        
        var @event = MatchConfirmed.Create(Id, DateTime.UtcNow);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(MatchConfirmed @event)
    {
        Version++;
        MatchStatus = MatchStatus.Confirmed;
    }

    public void AssignTossWinner(Guid tossWinnerId)
    {
        if(MatchStatus == MatchStatus.Confirmed || MatchStatus == MatchStatus.Live)
        {
            var @event = TossWinnerAssigned.Create(Id, tossWinnerId);

            Enqueue(@event);
            Apply(@event);
        }
        else
        {
            throw new InvalidOperationException($"Assigning Toss Winner For Match in '{MatchStatus}' status is not allowed.");
        }
    }

    public void Apply(TossWinnerAssigned @event)
    {
        Version++;
        TossWinnerId = @event.TossWinnerId;
    }

    public void GoLive()
    {
        if(MatchStatus != MatchStatus.Confirmed)
            throw new InvalidOperationException($"Setting Match as Live in '{MatchStatus}' status is not allowed.");
        
        var @event = LiveMatchAnnounced.Create(Id, DateTime.UtcNow);

        Enqueue(@event);
        Apply(@event);
    }

    public void Apply(LiveMatchAnnounced @event)
    {
        Version++;
        MatchStatus = MatchStatus.Live;
    }

    private Player? FindPlayerItemMatchingWith(Player player)
    {
        return Players
            .SingleOrDefault(p => p.MatchesPlayer(player));
    }
}