using Overs.Over.Ball;

namespace Overs.Over.RecordingBall;
public record BallRecorded (
    Guid OverId,
    OverBall Ball
)
{
    public static BallRecorded Create(Guid overId, OverBall overBall)
    {
        if(overId == null || overId == Guid.Empty)
            throw new ArgumentOutOfRangeException(nameof(overId));
        if(overBall == null)
            throw new ArgumentNullException(nameof(overBall));
        return new BallRecorded(overId, overBall);
    }
}