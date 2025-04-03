using DrawnUi.Draw;
using System.ComponentModel;

namespace DrawnUi.Controls;

/// <summary>
/// Base ISkiaCell implementation
/// </summary>
public class SkiaDrawnCell : SkiaLayout, ISkiaCell
{
    protected virtual void SetContent()
    {

    }

    public virtual void OnScrolled()
    {

    }

    public virtual TouchActionEventHandler LongPressingHandler => (sender, args) =>
    {
        args.PreventDefault = true;
    };

    private bool _isAttaching;


    protected INotifyPropertyChanged _lastContext;

    public override void OnDisposing()
    {
        base.OnDisposing();

        FreeContext();
    }

    protected virtual void FreeContext()
    {
        _lastContext = null;
    }

    protected virtual void AttachContext()
    {
        if (BindingContext != null)
        {
            _lastContext = BindingContext as INotifyPropertyChanged;
        }
    }

    public override void ApplyBindingContext()
    {
        base.ApplyBindingContext();

        if (BindingContext != _lastContext && !_isAttaching)
        {
            _isAttaching = true;

            FreeContext();

            if (_lastContext == null)
            {
                SetContent();
                AttachContext();
            }
            _isAttaching = false;
        }
    }






}