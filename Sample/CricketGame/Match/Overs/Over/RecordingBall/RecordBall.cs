using Core.Commands;
using Overs.Over.Ball;

namespace Overs.Over.RecordingBall;
public record RecordBall (
    Guid OverId,
    OverBall Ball
): ICommand
{
    public static RecordBall Create(Guid overId, OverBall overBall)
    {
        if(overId == null || overId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(overId));
        if(overBall == null)
            throw new ArgumentNullException(nameof(overBall));
        return new RecordBall(overId, overBall);
    }
}