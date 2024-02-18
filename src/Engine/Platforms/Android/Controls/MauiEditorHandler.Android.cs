using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views.InputMethods;
using AndroidX.AppCompat.Widget;
using Microsoft.Maui.Handlers;
using System.Diagnostics.CodeAnalysis;
using TextChangedEventArgs = Android.Text.TextChangedEventArgs;

namespace DrawnUi.Maui.Controls;

public partial class MauiEditorHandler : EditorHandler
{
    AppCompatEditText _control;

    protected override void ConnectHandler(AppCompatEditText platformView)
    {
        base.ConnectHandler(platformView);

        _control = platformView;
        //platformView.EditorAction += OnEditorAction;

        platformView.TextChanged += OnTextChanged;

        ApplySettings();
    }

    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        Android.Graphics.Rect visibleRect = new();
        _control.GetLocalVisibleRect(visibleRect);

        if (_control.Layout != null)
        {
            var scrollAmount = _control.Layout.GetLineTop(_control.LineCount) - _control.Height;
            // if there is no need to scroll, scrollAmount will be <=0
            if (scrollAmount > 0)
                _control.ScrollTo(0, scrollAmount);
            else
                _control.ScrollTo(0, 0);
        }
    }

    void ApplySettings()
    {
        if (_control != null)
        {
            _control.SetPadding(0, 0, 0, 0);
            _control.VerticalScrollBarEnabled = true;
            _control.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
        }
    }

    public override void PlatformArrange(Rect frame)
    {
        base.PlatformArrange(frame);

        ApplySettings();
    }

    protected override void DisconnectHandler(AppCompatEditText platformView)
    {
        _control = null;

        platformView.TextChanged -= OnTextChanged;

        base.DisconnectHandler(platformView);
        //platformView.EditorAction -= OnEditorAction;
    }

    protected override AppCompatEditText CreatePlatformView()
    {
        return new SubclassedAppCompatEditText(Context);

        //return base.CreatePlatformView();
    }

    public class SubclassedAppCompatEditText : AppCompatEditText
    {
        protected SubclassedAppCompatEditText(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        { }

        public SubclassedAppCompatEditText([NotNull] Context context) : base(context)
        { }

        public SubclassedAppCompatEditText([NotNull] Context context, IAttributeSet attrs) : base(context, attrs)
        { }

        public SubclassedAppCompatEditText([NotNull] Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs, defStyleAttr)
        { }

        public override IInputConnection OnCreateInputConnection(EditorInfo outAttrs)
        {
            var conn = base.OnCreateInputConnection(outAttrs);

            outAttrs.ImeOptions &= ~Android.Views.InputMethods.ImeFlags.NoEnterAction;

            return conn;
        }
    }
}