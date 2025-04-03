using Android.Text;
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
        mapper.ReplaceMapping<IEntry, IEntryHandler>(Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific.Entry.ImeOptionsProperty.PropertyName, MapImeOptions);

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

    AppCompatEditText _control;

    protected MauiEntry Control => VirtualView as MauiEntry;

    protected override void ConnectHandler(AppCompatEditText platformView)
    {
        base.ConnectHandler(platformView);

        _control = platformView;

        platformView.EditorAction += OnEditorAction;
        platformView.TextChanged += OnTextChanged;

        //ApplySettings();
    }

    void ApplySettings()
    {
        if (_control != null && MainThread.IsMainThread)
        {
            _control.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
            _control.SetPadding(0, 0, 0, 0);

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
                _control.SetHorizontallyScrolling(false);
            }
            else
            {
                _implyMaxLines = 1;
                _control.SetHorizontallyScrolling(true);
            }

            _control.SetMaxLines(_implyMaxLines);
            _control.RequestLayout();
            _control.Invalidate();
        }

    }

    private int _implyMaxLines;

    public override void PlatformArrange(Rect frame)
    {
        base.PlatformArrange(frame);

        //ApplySettings();
        if (Control != null)
            Control.Keyboard = Keyboard.Numeric;

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

    protected override AppCompatEditText CreatePlatformView()
    {
        _control = base.CreatePlatformView();

        _control.SetPadding(0, 0, 0, 0);
        _control.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);

        // ApplySettings();

        return _control;


        _control.InputType = InputTypes.TextVariationNormal;
        _control.ImeOptions = ImeAction.Done;

        _control.Gravity = GravityFlags.Top;
        _control.TextAlignment = Android.Views.TextAlignment.ViewStart;
        _control.SetSingleLine(false);

        //created.SetAutoSizeTextTypeWithDefaults(AutoSizeTextType.Uniform);
        //_control.LayoutParameters = new ViewGroup.LayoutParams(
        //    ViewGroup.LayoutParams.MatchParent,
        //    ViewGroup.LayoutParams.WrapContent);
        _control.VerticalScrollBarEnabled = true;
        _control.ScrollBarStyle = ScrollbarStyles.InsideInset;
        _control.OverScrollMode = OverScrollMode.Always;

        _control.SetPadding(0, 0, 0, 0);
        _control.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);

        // ApplySettings();

        return _control;
    }

    private void OnTextChanged(object sender, Android.Text.TextChangedEventArgs e)
    {
        Android.Graphics.Rect visibleRect = new();
        _control.GetLocalVisibleRect(visibleRect);

        //if (Control.MaxLines == 1)
        //{
        //    _control.SetMaxLines(1);
        //}

        Debug.WriteLine($"IsSingleLine {_control.IsSingleLine} lines {_control.MaxLines} gravity {_control.Gravity} inputType {_control.InputType}");

        if (_control.Layout != null)
        {
            //need this to apply our parent control new size to dynamic "Layout" property
            _control.RequestLayout();
            _control.Invalidate();

            int x = 0, y = 0;

            var scrollX = _control.ScrollX; // Current horizontal scroll position
            var contentWidth = _control.Layout.GetLineWidth(0); // Width of the content
            var controlWidth = _control.Width - _control.PaddingLeft - _control.PaddingRight; // Width of the control

            var scrollAmount = contentWidth - (controlWidth + scrollX);
            if (scrollAmount > 0)
            {
                x = (int)scrollAmount;
            }

            var scrollY = _control.Layout.GetLineTop(_control.LineCount) - _control.Height;
            // if there is no need to scroll, scrollAmount will be <=0
            if (scrollY > 0)
                y = scrollY;

            _control.ScrollTo(x, y);
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
        if (_control == null)
            return;

        var previousCursorPosition = _control.SelectionStart;
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

        _control.InputType = inputType;
        _control.SetMaxLines(_implyMaxLines);

        if (keyboard is not CustomKeyboard)
        {
            UpdateIsTextPredictionEnabled(_control, Control);
            UpdateIsSpellCheckEnabled(_control, Control);
        }

        if (keyboard == Keyboard.Numeric)
        {
            _control.KeyListener = LocalizedDigitsKeyListener.Create(_control.InputType);
        }

        if (Control is IElement element)
        {
            var services = element.Handler?.MauiContext?.Services;

            if (services == null)
                return;

            var fontManager = services.GetRequiredService<IFontManager>();
            _control.UpdateFont(Control, fontManager);
        }

        // If we implement the OnSelectionChanged method, this method is called after a keyboard layout change with SelectionStart = 0,
        // Let's restore the cursor position to its previous location.
        _control.SetSelection(previousCursorPosition);
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

