using AppoMobi.Specials;
using DrawnUi.Maui.Views;
using Sandbox.Views;

namespace Sandbox;

/// <summary>
/// Helper for code-behind HotReload
/// </summary>
public class BasePageWithCanvas : BasePage, IDisposable
{
    protected Canvas Canvas;

    public override void Dispose()
    {
        base.Dispose();

        this.Content = null;
        Canvas?.Dispose();
    }

    private void ReloadUI(Type[] obj)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            try
            {
                Canvas?.Dispose();
                Canvas = null;
                this.Content = CreateCanvas();
            }
            catch (Exception e)
            {
                Super.DisplayException(this, e);
            }
        });
    }

    public BasePageWithCanvas()
    {
#if DEBUG
        Super.HotReload += ReloadUI;
#endif

        Tasks.StartDelayed(TimeSpan.FromMilliseconds(1), () =>
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                try
                {
                    this.Content = CreateCanvas();
                }
                catch (Exception e)
                {
                    Super.DisplayException(this, e);
                }
            });
        });

    }

    public virtual Canvas CreateCanvas()
    {
        return Canvas;
    }
}
