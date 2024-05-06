using Android.Content;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using System.Diagnostics.CodeAnalysis;

namespace DrawnUi.Maui.Controls;

public partial class MauiEditorHandlerBack : EditorHandler
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


    /*
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
    */

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

    private bool once;
    void ApplySettings()
    {
        if (_control != null)
        {
            _control.SetPadding(0, 0, 0, 0);
            _control.VerticalScrollBarEnabled = true;
            _control.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);

            _control.LayoutParameters = new ViewGroup.LayoutParams(
                ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent);

            _control.Gravity = Android.Views.GravityFlags.Top | Android.Views.GravityFlags.Start;

            _control.SetAutoSizeTextTypeWithDefaults(AutoSizeTextType.None); ;

            _control.SetSingleLine(false);
            _control.InputType = Android.Text.InputTypes.ClassText |
                                 Android.Text.InputTypes.TextFlagMultiLine |
                                 Android.Text.InputTypes.TextFlagCapSentences;
        }
    }

    public override void PlatformArrange(Rect frame)
    {
        base.PlatformArrange(frame);

        ApplySettings();
    }



    public override void SetVirtualView(IView view)
    {
        base.SetVirtualView(view);

        // TODO: NET8 issoto - Remove the casting once we can set the TPlatformView generic type as MauiAppCompatEditText
        if (!once && PlatformView is SubclassedAppCompatEditText editText)
            editText.SelectionChanged += MyOnSelectionChanged;

        once = true;
    }

    private void MyOnSelectionChanged(object? sender, EventArgs e)
    {
        var cursorPosition = PlatformView.GetCursorPosition();
        var selectedTextLength = PlatformView.GetSelectedTextLength();

        if (VirtualView.CursorPosition != cursorPosition)
            VirtualView.CursorPosition = cursorPosition;

        if (VirtualView.SelectionLength != selectedTextLength)
            VirtualView.SelectionLength = selectedTextLength;
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
        var editText = new SubclassedAppCompatEditText(Context)
        {
            ImeOptions = ImeAction.Done,
            Gravity = GravityFlags.Top,
            TextAlignment = Android.Views.TextAlignment.ViewStart,
        };

        editText.SetSingleLine(false);
        editText.SetHorizontallyScrolling(false);

        return editText;
    }

    public class SubclassedAppCompatEditText : AppCompatEditText
    {
        protected override void OnSelectionChanged(int selStart, int selEnd)
        {
            base.OnSelectionChanged(selStart, selEnd);

            SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler? SelectionChanged;

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

public static class EditTextExtensions
{
    public static int GetCursorPosition(this EditText editText, int cursorOffset = 0)
    {
        var newCursorPosition = editText.SelectionStart + cursorOffset;
        return Math.Max(0, newCursorPosition);
    }

    public static int GetSelectedTextLength(this EditText editText)
    {
        var selectedLength = editText.SelectionEnd - editText.SelectionStart;
        return Math.Max(0, selectedLength);
    }


}