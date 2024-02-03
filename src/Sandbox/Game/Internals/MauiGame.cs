using DrawnUi.Maui;
using DrawnUi.Maui;
using DrawnUi.Maui;

namespace SpaceShooter.Game;

/// <summary>
/// Base class for implementing a game. StartLoop, StopLoop, override GameLoop(..) etc.
/// </summary>
public class MauiGame : SkiaLayout
{

    private ActionOnTickAnimator _appLoop;
    protected long LastFrameTimeNanos;

    public MauiGame()
    {
        KeyboardManager.KeyDown += OnKeyboardDownEvent;
        KeyboardManager.KeyUp += OnKeyboardUpEvent;
    }

    ~MauiGame()
    {
        KeyboardManager.KeyUp -= OnKeyboardUpEvent;
        KeyboardManager.KeyDown -= OnKeyboardDownEvent;
    }


    /// <summary>
    /// Override this for your game. `deltaMs` is time elapsed between the previous frame and this one 
    /// </summary>
    /// <param name="deltaMs"></param>
    public virtual void GameLoop(float deltaMs)
    {

    }

    /// <summary>
    /// Stops game loop
    /// </summary>
    public virtual void StopLoop()
    {
        _appLoop.Stop();
    }

    /// <summary>
    /// Starts game loop
    /// </summary>
    /// <param name="delayMs"></param>
    public void StartLoop(int delayMs = 0)
    {
        if (_appLoop == null)
        {
            _appLoop = new(this, (t) => Task.Run(() => GameTick(t)).ConfigureAwait(false));
        }
        _appLoop.Start(delayMs);
    }

    object lockTick = new();

    /// <summary>
    /// Internal, use override GameLoop for your game.
    /// </summary>
    /// <param name="frameTime"></param>
    protected virtual void GameTick(long frameTime)
    {
        lock (lockTick)
        {
            // Incoming frameTime is in nanoseconds
            // Calculate delta time in seconds for later use
            float deltaTime = (frameTime - LastFrameTimeNanos) / 1_000_000_000.0f;
            LastFrameTimeNanos = frameTime;

            GameLoop(deltaTime);
        }
    }

    #region KEYS

    /// <summary>
    /// Override this to process game keys
    /// </summary>
    /// <param name="key"></param>
    public virtual void OnKeyDown(MauiKey key)
    {

    }

    /// <summary>
    /// Override this to process game keys
    /// </summary>
    /// <param name="key"></param>
    public virtual void OnKeyUp(MauiKey key)
    {

    }


    /// <summary>
    /// Do not use directly. It's public to be able to send keys to game manually if needed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="key"></param>
    public void OnKeyboardDownEvent(object sender, MauiKey key)
    {
        OnKeyDown(key);
    }

    /// <summary>
    /// Do not use directly. It's public to be able to send keys to game manually if needed.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="key"></param>
    public void OnKeyboardUpEvent(object sender, MauiKey key)
    {
        OnKeyUp(key);
    }


    #endregion

}