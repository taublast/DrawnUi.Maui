using DrawnUi.Draw;

namespace DrawnUi.Gaming
{
    public interface IMauiGame
    {
        void OnKeyDown(MauiKey key);
        void OnKeyUp(MauiKey key);

        void Pause();
        void Resume();
        void StopLoop();
        void StartLoop(int delayMs = 0);
    }
}
