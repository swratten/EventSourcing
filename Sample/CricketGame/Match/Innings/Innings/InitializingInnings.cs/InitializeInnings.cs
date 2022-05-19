using Core.Commands;
using Innings.Innings.Batsmen;
using Innings.Innings.Players;
using MediatR;

namespace Innings.Innings.InitializingInnings;

public record InitializeInnings (
    Guid MatchId,
    Guid BattingTeamId,
    int InningsNumber,
    int MaxOvers,
    int DeliveriesPerOver,
    bool TieBreaker,
    int TargetScore,
    IReadOnlyList<Batsman> Batsmen
) : ICommand
{
    public static InitializeInnings Create(
        Guid? matchId,
        Guid? battingTeamId,
        int? inningsNumber,
        int? maxOvers,
        int? deliveriesPerOver,
        bool? tieBreaker,
        int? targetScore,
        IReadOnlyList<Batsman>? batsmen
    )
    {
        if(!matchId.HasValue)
            throw new ArgumentNullException(nameof(matchId));
        if(!battingTeamId.HasValue)
            throw new ArgumentNullException(nameof(battingTeamId));
        if(!inningsNumber.HasValue)
            throw new ArgumentNullException(nameof(inningsNumber));
        if(!maxOvers.HasValue)
            throw new ArgumentNullException(nameof(maxOvers));
        if(!deliveriesPerOver.HasValue)
            throw new ArgumentNullException(nameof(deliveriesPerOver));
        if(!tieBreaker.HasValue)
            throw new ArgumentNullException(nameof(tieBreaker));
        if(!targetScore.HasValue)
            throw new ArgumentNullException(nameof(targetScore));
        if(batsmen == null)
            throw new ArgumentNullException(nameof(batsmen));
        
        return new InitializeInnings(matchId.Value, battingTeamId.Value, inningsNumber.Value, maxOvers.Value, deliveriesPerOver.Value, tieBreaker.Value, targetScore.Value, batsmen);
    }
}