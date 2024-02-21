using AppoMobi.Specials;
using DrawnUi.Maui.Draw;
using System.Windows.Input;

namespace DrawnUi.Maui.Controls;

/// <summary>
/// Used to draw maui element over a skia canvas. Positions elelement using drawnUi layout and sometimes just renders element bitmap snapshot instead of displaying the real element, for example, when scrolling/animating.
/// </summary>
public class SkiaMauiEntry : SkiaMauiElement, ISkiaGestureListener
{

    public SkiaMauiEntry()
    {

    }

    public override void SetNativeVisibility(bool state)
    {
        base.SetNativeVisibility(state);

        if (state && IsFocused)
        {
            SetFocusInternal(true);
        }
    }

    #region CAN LOCALIZE

    public static string ActionGo = "Go";
    public static string ActionNext = "Next";
    public static string ActionSend = "Send";
    public static string ActionSearch = "Search";
    public static string ActionDone = "Done";

    #endregion

    #region EVENTS

    public event EventHandler<string> TextChanged;

    public event EventHandler<bool> FocusChanged;

    public event EventHandler<string> TextSubmitted;

    #endregion

    public override bool IsClippedToBounds => true;

    protected virtual Entry GetOrCreateControl()
    {
        if (Control == null)
        {
            Control = new MauiEntry()
            {
                //BackgroundColor = Colors.Red
            };
            SubscribeToControl(true);
            UpdateControl();
            Content = Control;

            if (IsFocused)
                SetFocusInternal(true);
        }
        return Control;
    }

    public Entry Control { get; protected set; }

    protected void FocusNative()
    {
        //Debug.WriteLine($"[SKiaMauiEntry] Focusing native control..");
        Control.Focus();
    }

    protected void UnfocusNative()
    {
        //Debug.WriteLine($"[SKiaMauiEntry] Unfocusing native control..");
        IsFocused = false;
        //Control.Unfocus();
    }

    public void SetFocus(bool focus)
    {
        if (Control != null)
        {
            if (focus)
            {
                FocusNative();
            }
            else
            {
                UnfocusNative();
            }

            UpdateControl();
        }
    }


    protected virtual void AdaptControlSize()
    {
        if (Control != null)
        {
            Control.WidthRequest = this.Width;
            Control.HeightRequest = this.Height;
        }
    }

    object lockAccess = new();
    public virtual void UpdateControl()
    {
        if (Control != null && Control.Handler != null && Control.Handler.PlatformView != null)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                lock (lockAccess)
                {
                    var alias = SkiaFontManager.GetRegisteredAlias(this.FontFamily, this.FontWeight);
                    Control.FontFamily = alias;
                    Control.FontSize = FontSize;
                    Control.TextColor = this.TextColor;
                    Control.ReturnType = this.ReturnType;
                    Control.Text = Text;

                    Update();
                }

                AdaptControlSize();
            });
        }
    }

    protected override void OnLayoutChanged()
    {
        base.OnLayoutChanged();

        AdaptControlSize();
    }

    protected void SubscribeToControl(bool subscribe)
    {
        if (Control != null)
        {
            if (subscribe)
            {
                Control.Unfocused += OnControlUnfocused;
                Control.Focused += OnControlFocused;
                Control.TextChanged += OnControlTextChanged;
                Control.Completed += OnControlCompleted;
            }
            else
            {
                Control.Unfocused -= OnControlUnfocused;
                Control.Focused -= OnControlFocused;
                Control.TextChanged -= OnControlTextChanged;
                Control.Completed -= OnControlCompleted;
            }
        }
    }

    private void OnControlCompleted(object sender, EventArgs e)
    {
        if (!LockFocus)
        {
            Control.Unfocus();
        }
        TextSubmitted?.Invoke(this, Text);
        CommandOnSubmit?.Execute(Text);
    }

    private void OnControlTextChanged(object sender, TextChangedEventArgs e)
    {
        this.Text = Control.Text;
    }

    static object lockFocus = new();

    /// <summary>
    /// Invoked by Maui control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnControlFocused(object sender, FocusEventArgs e)
    {
        //Debug.WriteLine($"[SKiaMauiEntry] Focused by native");
        lock (lockFocus)
        {
            Superview.FocusedChild = this;
        }
    }

    /// <summary>
    /// Invoked by Maui control
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnControlUnfocused(object sender, FocusEventArgs e)
    {
        //Debug.WriteLine($"[SKiaMauiEntry] Unfocused by native");
        lock (lockFocus)
        {
            if (Superview.FocusedChild == this)
            {
                Superview.FocusedChild = null;
            };
        }
    }

    /// <summary>
    /// Called by DrawnUI when the focus changes
    /// </summary>
    /// <param name="focus"></param>
    public void OnFocusChanged(bool focus)
    {
        lock (lockFocus)
        {
            if (Control != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (!focus)
                        UnfocusNative();
                    else
                        FocusNative();
                });
            }
        }
    }

    public override void OnDisposing()
    {
        if (Control != null)
        {
            SubscribeToControl(false);
            Control.DisposeControlAndChildren();
            Control = null;
        }
        base.OnDisposing();
    }

    public override ScaledSize Measure(float widthConstraint, float heightConstraint, float scale)
    {
        GetOrCreateControl();

        return base.Measure(widthConstraint, heightConstraint, scale);
    }

    protected void SetFocusInternal(bool value)
    {
        if (Control != null)
        {
            if (!Control.IsFocused && value || Control.IsFocused && !value)
            {
                Tasks.StartDelayed(TimeSpan.FromMilliseconds(100), () =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (value)
                            FocusNative();
                        else
                            UnfocusNative();
                    });
                });
            }
        }
    }

    private static void OnNeedUpdateText(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaMauiEntry control)
        {
            control.UpdateControl();
        }
    }

    /// <summary>
    /// TODO
    /// </summary>
    bool LockFocus { get; set; }


    #region   PROPERTIES


    private static void OnControlTextChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        //Debug.WriteLine($"[ENTRY] OnControlTextChanged!");
        if (bindable is SkiaMauiEntry control)
        {
            control.TextChanged?.Invoke(control, (string)newvalue);
            control.CommandOnTextChanged?.Execute((string)newvalue);
            OnNeedUpdateText(bindable, oldvalue, newvalue);
        }
    }

    public static readonly BindableProperty FontFamilyProperty = BindableProperty.Create(nameof(FontFamily),
       typeof(string), typeof(SkiaMauiEntry), string.Empty, propertyChanged: OnNeedUpdateText);
    public string FontFamily
    {
        get { return (string)GetValue(FontFamilyProperty); }
        set { SetValue(FontFamilyProperty, value); }
    }

    public static readonly BindableProperty TextColorProperty = BindableProperty.Create(
        nameof(TextColor), typeof(Color), typeof(SkiaMauiEntry),
        Colors.GreenYellow,
        propertyChanged: OnNeedUpdateText);
    public Color TextColor
    {
        get { return (Color)GetValue(TextColorProperty); }
        set { SetValue(TextColorProperty, value); }
    }

    public static readonly BindableProperty FontWeightProperty = BindableProperty.Create(
        nameof(FontWeight),
        typeof(int),
        typeof(SkiaMauiEntry),
        0,
        propertyChanged: OnNeedUpdateText);

    public int FontWeight
    {
        get { return (int)GetValue(FontWeightProperty); }
        set { SetValue(FontWeightProperty, value); }
    }

    public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(nameof(FontSize),
        typeof(double), typeof(SkiaMauiEntry), 12.0,
        propertyChanged: OnNeedUpdateText);

    public double FontSize
    {
        get { return (double)GetValue(FontSizeProperty); }
        set { SetValue(FontSizeProperty, value); }
    }

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text),
        typeof(string),
        typeof(SkiaMauiEntry),
        default(string),
        BindingMode.TwoWay,
        propertyChanged: OnControlTextChanged);


    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    public static readonly BindableProperty ReturnTypeProperty = BindableProperty.Create(
        nameof(ReturnType),
        typeof(ReturnType),
        typeof(SkiaMauiEntry),
        ReturnType.Done);

    public ReturnType ReturnType
    {
        get { return (ReturnType)GetValue(ReturnTypeProperty); }
        set { SetValue(ReturnTypeProperty, value); }
    }

    public static readonly BindableProperty CommandOnSubmitProperty = BindableProperty.Create(
        nameof(CommandOnSubmit),
        typeof(ICommand),
        typeof(SkiaMauiEntry),
        null);

    public ICommand CommandOnSubmit
    {
        get { return (ICommand)GetValue(CommandOnSubmitProperty); }
        set { SetValue(CommandOnSubmitProperty, value); }
    }

    public static readonly BindableProperty CommandOnFocusChangedProperty = BindableProperty.Create(
        nameof(CommandOnFocusChanged),
        typeof(ICommand),
        typeof(SkiaMauiEntry),
        null);

    public ICommand CommandOnFocusChanged
    {
        get { return (ICommand)GetValue(CommandOnFocusChangedProperty); }
        set { SetValue(CommandOnFocusChangedProperty, value); }
    }

    public static readonly BindableProperty CommandOnTextChangedProperty = BindableProperty.Create(
        nameof(CommandOnTextChanged),
        typeof(ICommand),
        typeof(SkiaMauiEntry),
        null);

    public ICommand CommandOnTextChanged
    {
        get { return (ICommand)GetValue(CommandOnTextChangedProperty); }
        set { SetValue(CommandOnTextChangedProperty, value); }
    }

    public new static readonly BindableProperty IsFocusedProperty = BindableProperty.Create(
        nameof(IsFocused),
        typeof(bool),
        typeof(SkiaMauiEntry),
        false,
        BindingMode.TwoWay,
        propertyChanged: OnNeedChangeFocus);

    private static void OnNeedChangeFocus(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaMauiEntry control)
        {
            control.SetFocusInternal((bool)newvalue);
        }
    }

    public new bool IsFocused
    {
        get { return (bool)GetValue(IsFocusedProperty); }
        set { SetValue(IsFocusedProperty, value); }
    }

    #endregion

}