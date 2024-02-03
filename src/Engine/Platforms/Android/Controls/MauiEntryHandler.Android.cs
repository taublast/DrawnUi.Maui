using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using AndroidX.AppCompat.Widget;
using Microsoft.Maui.Handlers;

namespace DrawnUi.Maui.Controls;


public partial class MauiEntryHandler : EntryHandler
{
    AppCompatEditText _control;

    protected override void ConnectHandler(AppCompatEditText platformView)
    {
        base.ConnectHandler(platformView);


        _control = platformView;

        platformView.EditorAction += OnEditorAction;
        platformView.TextChanged += OnTextChanged;
    }

    public override void PlatformArrange(Rect frame)
    {
        base.PlatformArrange(frame);

        _control.SetPadding(0, 0, 0, 0);
        _control.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
    }

    protected override void DisconnectHandler(AppCompatEditText platformView)
    {
        base.DisconnectHandler(platformView);

        _control = null;

        platformView.EditorAction -= OnEditorAction;
        platformView.TextChanged -= OnTextChanged;
    }



    private void OnEditorAction(object sender, Android.Widget.TextView.EditorActionEventArgs e)
    {
        var actionId = e.ActionId;
        var evt = e.Event;

        if (actionId == ImeAction.Done ||
            (actionId == ImeAction.ImeNull && evt?.KeyCode == Keycode.Enter && evt?.Action == KeyEventActions.Up))
            return; //Already handled by base class.

        if (actionId != ImeAction.ImeNull)
            VirtualView?.Completed();

        e.Handled = true;
    }

    private void OnTextChanged(object sender, Android.Text.TextChangedEventArgs e)
    {
        Android.Graphics.Rect visibleRect = new();
        _control.GetLocalVisibleRect(visibleRect);

        if (_control.Layout != null)
        {
            var scrollX = _control.ScrollX; // Current horizontal scroll position
            var contentWidth = _control.Layout.GetLineWidth(0); // Width of the content
            var controlWidth = _control.Width - _control.PaddingLeft - _control.PaddingRight; // Width of the control

            // Calculate the amount to scroll
            var scrollAmount = contentWidth - (controlWidth + scrollX);
            // Scroll only if there is overflow
            if (scrollAmount > 0)
            {
                _control.ScrollTo((int)scrollAmount, 0);
            }
            else
            {
                _control.ScrollTo(0, 0);
            }
        }
    }



}

