using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Controls.Platform.Android;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using Microsoft.Maui.Handlers;


namespace DrawnUi.Maui.Controls;


public partial class MauiEntryHandler : EntryHandler
{
    AppCompatEditText _control;

    protected MauiEntry Control => VirtualView as MauiEntry;

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

        handler.PlatformView.ImeOptions = ImeAction.Unspecified;// imeOptions;
    }

    public class MyMapper : PropertyMapper<IEntry, IEntryHandler>
    {
        public MyMapper(params IPropertyMapper[] chained) : base(chained)
        {

        }

        protected override void UpdatePropertyCore(string key, IElementHandler viewHandler, IElement virtualView)
        {
            Debug.WriteLine($"[MAPPER] update {key}");
            base.UpdatePropertyCore(key, viewHandler, virtualView);
        }

        protected override void SetPropertyCore(string key, Action<IElementHandler, IElement> action)
        {
            Debug.WriteLine($"[MAPPER] set {key}");
            base.SetPropertyCore(key, action);
        }

    }

    protected override void ConnectHandler(AppCompatEditText platformView)
    {
        base.ConnectHandler(platformView);

        var mapper = new MyMapper(Mapper);
        Mapper = mapper;

        Mapper.ReplaceMapping<IEntry, IEntryHandler>(nameof(MauiEntry.MaxLines), MapAllSettings);
        Mapper.ReplaceMapping<IEntry, IEntryHandler>(Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific.Entry.ImeOptionsProperty.PropertyName, MapImeOptions);

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
                _control.SetSingleLine(false);

                _control.InputType = Android.Text.InputTypes.ClassText |
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
                    _control.Ellipsize = null;
                    _control.LayoutParameters = new ViewGroup.LayoutParams(
                        ViewGroup.LayoutParams.MatchParent,
                        ViewGroup.LayoutParams.WrapContent);
                }
            }
            else
            {
                _implyMaxLines = 1;
                _control.SetSingleLine(true);

                //_control.InputType = InputTypes.TextFlagMultiLine;
            }

            _control.SetMaxLines(_implyMaxLines);
        }
    }

    private int _implyMaxLines;

    public override void PlatformArrange(Rect frame)
    {
        base.PlatformArrange(frame);

        ApplySettings();
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
        var created = base.CreatePlatformView();

        _control = created;

        ApplySettings();

        return created;
    }

    private void OnTextChanged(object sender, Android.Text.TextChangedEventArgs e)
    {
        Android.Graphics.Rect visibleRect = new();
        _control.GetLocalVisibleRect(visibleRect);

        if (_control.Layout != null)
        {
            if (_implyMaxLines != _control.MaxLines)
            {
                ApplySettings();
            }

            if (_control.Layout == null)
                return;

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

