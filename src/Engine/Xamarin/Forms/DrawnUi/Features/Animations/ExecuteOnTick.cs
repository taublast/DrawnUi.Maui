namespace DrawnUi.Draw;

/// <summary>
/// Just register this animator to run custom code on every frame creating a kind of game loop if needed.
/// </summary>
public class ActionOnTickAnimator : AnimatorBase
{
    private readonly Action<long> _action;

    public override bool TickFrame(long frameTime)
    {
        _action?.Invoke(frameTime);
        return base.TickFrame(frameTime);
    }

    public ActionOnTickAnimator(SkiaControl parent, Action<long> action) : base(parent)
    {
        _action = action;
    }
}