using Core.Commands;
using Overs.Over.BowlEnd;

namespace Overs.Over.InitializingOver;
public record InitializeOver(
    Guid OverId,
    Guid MatchId,
    Guid InningsId,
    Guid BattingTeamId,
    Batsman BatsmanOne,
    Batsman BatsmanTwo,
    int InningsNumber,
    int OverNumber,
    Guid BowlerId,
    Guid KeeperId,
    BowlingEnd BowlingEnd
): ICommand
{
    public static InitializeOver Create(
        Guid overId, 
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
        if(overId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(overId));
        if(matchId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(matchId));
        if(inningsId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(inningsId));
        if(battingTeamId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(battingTeamId));
        if(bowlerId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(bowlerId));
        if(keeperId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(keeperId));
        if(inningsNumber <=0)
            throw new ArgumentOutOfRangeException(nameof(inningsNumber));
        if(overNumber <=0)
            throw new ArgumentOutOfRangeException(nameof(overNumber));
        if(batsmanOne == null)
            throw new ArgumentNullException(nameof(batsmanOne));
        if(batsmanTwo == null)
            throw new ArgumentNullException(nameof(batsmanTwo));
        if(bowlingEnd == default)
            throw new ArgumentOutOfRangeException(nameof(bowlingEnd));
        return new InitializeOver(overId, matchId, inningsId, battingTeamId, batsmanOne, batsmanTwo, inningsNumber, overNumber, bowlerId, keeperId, bowlingEnd);
    }
}