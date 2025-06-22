using Android.Runtime;
using Android.Text;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Platform.Android;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using static Android.Text.Method.TextKeyListener;
using View = Android.Views.View;

namespace DrawnUi.Controls;

public partial class MauiEntryHandler : EntryHandler
{
    #region UNUSED

    static IPropertyMapper<IEntry, IEntryHandler> ChangeMapper()
    {
        var mapper = new MyMapper(Mapper);

        // UpdateReturnType UpdateKeyboard UpdateIsReadOnly UpdateIsPassword
        // UpdateReturnType UpdateKeyboard UpdateIsReadOnly UpdateIsPassword

        //if (key.IsEither("IsPassword",
        //        "IsReadOnly",
        //        "ReturnType",
        //        "Keyboard"))

        //[nameof(IEntry.IsPassword)] = MapIsPassword,
        //[nameof(IEntry.IsReadOnly)] = MapIsReadOnly,
        //[nameof(IEntry.Keyboard)] = MapKeyboard,
        //[nameof(IEntry.ReturnType)] = MapReturnType,

        //mapper.ReplaceMapping<IEntry, IEntryHandler>(nameof(IEntry.IsPassword), FixMapIsPassword);

        mapper.ReplaceMapping<IEntry, IEntryHandler>(nameof(MauiEntry.MaxLines), MapAllSettings);
        mapper.ReplaceMapping<IEntry, IEntryHandler>(
            Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific.Entry.ImeOptionsProperty.PropertyName,
            MapImeOptions);

        return mapper;
    }

    //public MauiEntryHandler() : this(ChangeMapper())
    //{

    //}

    //public MauiEntryHandler(IPropertyMapper? mapper)
    //    : base(mapper ?? ChangeMapper(), CommandMapper)
    //{

    //}

    //public MauiEntryHandler(IPropertyMapper? mapper, CommandMapper? commandMapper)
    //    : base(mapper ?? ChangeMapper(), commandMapper ?? CommandMapper)
    //{

    //}

    public static void MapAllSettings(IEntryHandler handler, IEntry entry)
    {
        if (handler is MauiEntryHandler me)
        {
            me.ApplySettings();
        }
    }

    public static void MapImeOptions(IEntryHandler handler, IEntry entry)
    {
        var imeOptions = ((Microsoft.Maui.Controls.Entry)entry).OnThisPlatform().ImeOptions().ToPlatform();

        handler.PlatformView.ImeOptions = imeOptions;
    }

    public class MyMapper : PropertyMapper<IEntry, IEntryHandler>
    {
        public MyMapper(params IPropertyMapper[] chained) : base(chained)
        {
        }
    }

    #endregion

    public AppCompatEditText NativeControl;

    protected MauiEntry Control => VirtualView as MauiEntry;


 

    //private class BackspaceInputConnection : InputConnectionWrapper
    //{
    //    private readonly MauiEntryHandler _renderer;

    //    protected BackspaceInputConnection(IntPtr javaReference, JniHandleOwnership transfer, MauiEntryHandler renderer) : base(javaReference, transfer)
    //    {
    //        _renderer = renderer;
    //    }

    //    public BackspaceInputConnection(IInputConnection target, bool mutable, MauiEntryHandler renderer) : base(target, mutable)
    //    {
    //        _renderer = renderer;
    //    }

    //    public override bool DeleteSurroundingText(int beforeLength, int afterLength)
    //    {
    //        if (beforeLength == 1 && afterLength == 0)
    //        {
    //            // Inject backspace event
    //            _renderer.NativeControl.DispatchKeyEvent(new KeyEvent(KeyEventActions.Down, Keycode.Del));
    //            _renderer.NativeControl.DispatchKeyEvent(new KeyEvent(KeyEventActions.Up, Keycode.Del));
    //        }
    //        return base.DeleteSurroundingText(beforeLength, afterLength);
    //    }
    //}

 
 
    protected override void ConnectHandler(AppCompatEditText platformView)
    {
        base.ConnectHandler(platformView);

        NativeControl = platformView;

        platformView.EditorAction += OnEditorAction;
        platformView.TextChanged += OnTextChanged;
        platformView.FocusChange += OnFocusChanged;

        //ApplySettings();
    }

    private void OnFocusChanged(object sender, View.FocusChangeEventArgs e)
    {
        if (!e.HasFocus)
            return;

        //todo create BackspaceInputConnection _connection if null

        var platformView = sender as AppCompatEditText;
        var keyboard = Control.Keyboard;

        //fricky fix
        if (keyboard == Keyboard.Numeric || keyboard == Keyboard.Telephone)
        {
            if (Control.IsPassword)
            {
                platformView.InputType = InputTypes.ClassNumber | InputTypes.NumberVariationPassword;
            }
            else
            {
                platformView.InputType = InputTypes.ClassNumber | InputTypes.NumberVariationNormal;
            }
        }
        else
        {
            if (Control.IsPassword)
            {
                platformView.InputType =
                    InputTypes.ClassText | InputTypes.TextFlagMultiLine | InputTypes.TextFlagNoSuggestions | InputTypes.TextVariationVisiblePassword;
            }
            else
            {
                platformView.InputType =
                    InputTypes.ClassText | InputTypes.TextFlagMultiLine | InputTypes.TextFlagNoSuggestions;
            }
        }
 
        Debug.WriteLine(
                $"[EntryHandler] Final InputType: {platformView.InputType}, KeyListener: {platformView.KeyListener?.GetType()?.Name}");
    }

    void ApplySettings()
    {
        if (NativeControl != null && MainThread.IsMainThread)
        {
            if (Control.Text != NativeControl.Text)
                NativeControl.Text = Control.Text;

            NativeControl.BackgroundTintList =
                Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
            NativeControl.SetPadding(0, 0, 0, 0);

            if (Control.MaxLines != 1)
            {
                if (Control.MaxLines > 1)
                {
                    _implyMaxLines = Control.MaxLines;
                }
                else
                {
                    _implyMaxLines = int.MaxValue;
                }

                NativeControl.SetHorizontallyScrolling(false);
            }
            else
            {
                _implyMaxLines = 1;
                NativeControl.SetHorizontallyScrolling(true);
            }

            NativeControl.SetMaxLines(_implyMaxLines);
            NativeControl.RequestLayout();
            NativeControl.Invalidate();
        }
    }

    private int _implyMaxLines;

    //public override void PlatformArrange(Rect frame)
    //{
    //    base.PlatformArrange(frame);

    //    //ApplySettings();
    //    if (Control != null)
    //        Control.Keyboard = Keyboard.Numeric;

    //}

    protected override void DisconnectHandler(AppCompatEditText platformView)
    {
        base.DisconnectHandler(platformView);

        NativeControl = null;

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

    protected override AppCompatEditText CreatePlatformView()
    {
        NativeControl = base.CreatePlatformView();

        NativeControl.SetPadding(0, 0, 0, 0);
        NativeControl.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);

        ApplySettings();

        return NativeControl;


        NativeControl.InputType = InputTypes.TextVariationNormal;
        NativeControl.ImeOptions = ImeAction.Done;

        NativeControl.Gravity = GravityFlags.Top;
        NativeControl.TextAlignment = Android.Views.TextAlignment.ViewStart;
        NativeControl.SetSingleLine(false);

        //created.SetAutoSizeTextTypeWithDefaults(AutoSizeTextType.Uniform);
        //_control.LayoutParameters = new ViewGroup.LayoutParams(
        //    ViewGroup.LayoutParams.MatchParent,
        //    ViewGroup.LayoutParams.WrapContent);
        NativeControl.VerticalScrollBarEnabled = true;
        NativeControl.ScrollBarStyle = ScrollbarStyles.InsideInset;
        NativeControl.OverScrollMode = OverScrollMode.Always;

        NativeControl.SetPadding(0, 0, 0, 0);
        NativeControl.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);

        // ApplySettings();

        return NativeControl;
    }

    private void OnTextChanged(object sender, Android.Text.TextChangedEventArgs e)
    {
        Android.Graphics.Rect visibleRect = new();
        NativeControl.GetLocalVisibleRect(visibleRect);

        //if (Control.MaxLines == 1)
        //{
        //    _control.SetMaxLines(1);
        //}

        Debug.WriteLine(
            $"Text: '{NativeControl.Text}' IsSingleLine {NativeControl.IsSingleLine} lines {NativeControl.MaxLines} gravity {NativeControl.Gravity} inputType {NativeControl.InputType}");

        return;

        try
        {
            if (NativeControl.Layout != null)
            {
                //need this to apply our parent control new size to dynamic "Layout" property
                NativeControl.RequestLayout();
                NativeControl.Invalidate();

                int x = 0, y = 0;

                var scrollX = NativeControl.ScrollX; // Current horizontal scroll position
                var contentWidth = NativeControl.Layout.GetLineWidth(0); // Width of the content
                var controlWidth =
                    NativeControl.Width - NativeControl.PaddingLeft - NativeControl.PaddingRight; // Width of the control

                var scrollAmount = contentWidth - (controlWidth + scrollX);
                if (scrollAmount > 0)
                {
                    x = (int)scrollAmount;
                }

                var scrollY = NativeControl.Layout.GetLineTop(NativeControl.LineCount) - NativeControl.Height;
                // if there is no need to scroll, scrollAmount will be <=0
                if (scrollY > 0)
                    y = scrollY;

                NativeControl.ScrollTo(x, y);
            }
        }
            catch (Exception exception)
            {
                Debug.WriteLine(exception);
            }
        }
 

    // UpdateReturnType UpdateKeyboard UpdateIsReadOnly UpdateIsPassword

    public static void UpdateIsTextPredictionEnabled(EditText editText, ITextInput textInput)
    {
        var keyboard = textInput.Keyboard;

        // TextFlagAutoCorrect will correct "Whats" -> "What's"
        // TextFlagAutoCorrect should not be confused with TextFlagAutocomplete
        // Autocomplete property pertains to fields that will "self-fill" - like an "Address" input box that fills with your saved data
        if (textInput.IsTextPredictionEnabled)
            editText.InputType |= InputTypes.TextFlagAutoCorrect;
        else
            editText.InputType &= ~InputTypes.TextFlagAutoCorrect;
    }

    public static void UpdateIsSpellCheckEnabled(EditText editText, ITextInput textInput)
    {
        // TextFlagNoSuggestions disables spellchecking (the red squiggly lines)
        if (!textInput.IsSpellCheckEnabled)
            editText.InputType |= InputTypes.TextFlagNoSuggestions;
        else
            editText.InputType &= ~InputTypes.TextFlagNoSuggestions;
    }

    public void SetInputType()
    {
        if (NativeControl == null)
            return;

        var previousCursorPosition = NativeControl.SelectionStart;
        var keyboard = Control.Keyboard;

        var inputType = keyboard.ToInputType();

        if (Control.MaxLines != 1)
        {
            //_control.SetSingleLine(false);

            inputType |=
                Android.Text.InputTypes.TextFlagMultiLine |
                Android.Text.InputTypes.TextFlagCapSentences;

            //_control.SetRawInputType(Android.Text.InputTypes.ClassText);

            if (Control.MaxLines > 1)
            {
                _implyMaxLines = Control.MaxLines;
            }
            else
            {
                _implyMaxLines = int.MaxValue;
            }
        }
        else
        {
            _implyMaxLines = 1;
            //_control.SetSingleLine(true);

            //_control.InputType = InputTypes.TextFlagMultiLine;
        }

        NativeControl.InputType = inputType;
        NativeControl.SetMaxLines(_implyMaxLines);

        if (keyboard is not CustomKeyboard)
        {
            UpdateIsTextPredictionEnabled(NativeControl, Control);
            UpdateIsSpellCheckEnabled(NativeControl, Control);
        }

        if (keyboard == Keyboard.Numeric)
        {
            NativeControl.KeyListener = LocalizedDigitsKeyListener.Create(NativeControl.InputType);
        }

        if (Control is IElement element)
        {
            var services = element.Handler?.MauiContext?.Services;

            if (services == null)
                return;

            var fontManager = services.GetRequiredService<IFontManager>();
            NativeControl.UpdateFont(Control, fontManager);
        }

        // If we implement the OnSelectionChanged method, this method is called after a keyboard layout change with SelectionStart = 0,
        // Let's restore the cursor position to its previous location.
        NativeControl.SetSelection(previousCursorPosition);
    }

    public static void FixMapIsPassword(IEntryHandler handler, IEntry entry)
    {
        handler.UpdateValue(nameof(IEntry.Text));

        //handler.PlatformView?.UpdateIsPassword(entry);
        if (handler is MauiEntryHandler myHandler)
        {
            myHandler.SetInputType();
        }
    }
}
