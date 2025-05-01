using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AppoMobi.Maui.DrawnUi.Draw;

public partial class SkiaControl
{
    #region STATES


    public bool DetectStatesChanged()
    {
        bool ret = DetectStatesChanged(this.States, this.InternalStates);
        if (ret)
            InternalStates = States.ToList();
        return ret;
    }
    protected bool DetectStatesChanged(IEnumerable<string> a, IEnumerable<string> b)
    {
        if (a == null || b == null)
            return true;

        if (b.Any(p => a.All(p2 => p2 != p)))
            return true;

        if (a.Any(p => b.All(p2 => p2 != p)))
            return true;

        return false;
    }

    public static readonly BindableProperty StatesProperty = BindableProperty.Create(nameof(States),
    typeof(IEnumerable<string>),
    typeof(SkiaControl),
    new[] { "Normal" }, propertyChanged: OnVisualStateChanged);

    private static void OnVisualStateChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is SkiaControl control)
        {
            //Debug.WriteLine($"[STATES] {control.GetType().Name} {control.States.ToList().ToTags()}");
            if (control.DetectStatesChanged())
            {
                control.ApplyStates(control.States);
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public IEnumerable<string> InternalStates { get; set; }


    public IEnumerable<string> States
    {
        get { return (IEnumerable<string>)GetValue(StatesProperty); }
        set { SetValue(StatesProperty, value); }
    }


    #endregion


    #region Styles

    public static readonly BindableProperty StylesProperty = BindableProperty.Create(
        nameof(Styles),
        typeof(IList<ConditionalStyle>),
        typeof(SkiaControl),
        defaultValueCreator: (instance) =>
        {
            var created = new ObservableCollection<ConditionalStyle>();
            StylesPropertyChanged(instance, null, created);
            return created;
        },
        validateValue: (bo, v) => v is IList<ConditionalStyle>,
        propertyChanged: StylesPropertyChanged);


    public IList<ConditionalStyle> Styles
    {
        get => (IList<ConditionalStyle>)GetValue(StylesProperty);
        set => SetValue(StylesProperty, value);
    }

    private static void StylesPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is SkiaControl skiaControl)
        {
            if (oldvalue is INotifyCollectionChanged oldCollection)
            {
                oldCollection.CollectionChanged -= skiaControl.OnStatesCollectionChanged;
            }

            if (newvalue is INotifyCollectionChanged newCollection)
            {
                newCollection.CollectionChanged += skiaControl.OnStatesCollectionChanged;
            }

            skiaControl.Update();
        }

    }

    private void OnStatesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
        case NotifyCollectionChangedAction.Add:
        foreach (ConditionalStyle newStates in e.NewItems)
        {
            newStates.SetParent(this);
            _weakViewsCollectionChangedSource.Raise(this, e);
        }

        break;

        case NotifyCollectionChangedAction.Reset:
        case NotifyCollectionChangedAction.Remove:
        foreach (ConditionalStyle oldStates in e.OldItems ?? new ConditionalStyle[0])
        {
            oldStates.SetParent(null);
            _weakViewsCollectionChangedSource.Raise(this, e);
        }

        break;
        }

        ApplyStates(this.States);
    }

    #endregion


}

public class ConditionalStyle : BindableObject
{
    public void SetParent(SkiaControl control)
    {
        Parent = control;
        BindingContext = control?.BindingContext;
    }
    public SkiaControl Parent { get; set; }

    public static readonly BindableProperty StateProperty = BindableProperty.Create(nameof(State),
        typeof(string),
        typeof(ConditionalStyle),
        null, propertyChanged: StylesPropertyChanged);
    public string State
    {
        get { return (string)GetValue(StateProperty); }
        set { SetValue(StateProperty, value); }
    }


    public static readonly BindableProperty ConditionProperty = BindableProperty.Create(nameof(Condition),
    typeof(bool),
    typeof(ConditionalStyle),
    false, propertyChanged: StylesBooleanChanged);
    public bool Condition
    {
        get { return (bool)GetValue(ConditionProperty); }
        set { SetValue(ConditionProperty, value); }
    }


    public static readonly BindableProperty StyleProperty = BindableProperty.Create(nameof(Style),
        typeof(Style),
        typeof(ConditionalStyle),
        null, propertyChanged: StylesPropertyChanged);
    public Style Style
    {
        get { return (Style)GetValue(StyleProperty); }
        set { SetValue(StyleProperty, value); }
    }


    private static void StylesBooleanChanged(BindableObject bindable, object oldvalue, object newvalue)
    {
        if (bindable is ConditionalStyle control)
        {
            control.Parent?.ApplyStyles();
        }
    }
    private static void StylesPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
    {

        //if (bindable is ConditionalStyle control)
        //{
        //    control.Parent?.ApplyStyles();
        //}
    }


    //#region SETTERS


    //public static readonly BindableProperty SettersProperty = BindableProperty.Create(
    //    nameof(Setters),
    //    typeof(IList<SkiaSetter>),
    //    typeof(SkiaVisualState),
    //    defaultValueCreator: (instance) =>
    //    {
    //        var created = new ObservableCollection<SkiaSetter>();
    //        SettersPropertyChanged(instance, null, created);
    //        return created;
    //    },
    //    validateValue: (bo, v) => v is IList<SkiaSetter>,
    //    propertyChanged: SettersPropertyChanged,
    //    coerceValue: CoerceSetters);


    //private readonly WeakEventSource<NotifyCollectionChangedEventArgs> _weakCollectionChangedSource = new();


    //public event EventHandler<NotifyCollectionChangedEventArgs> SettersWeakCollectionChanged
    //{
    //    add => _weakCollectionChangedSource.Subscribe(value);
    //    remove => _weakCollectionChangedSource.Unsubscribe(value);
    //}

    //public IList<SkiaSetter> Setters
    //{
    //    get => (IList<SkiaSetter>)GetValue(SettersProperty);
    //    set => SetValue(SettersProperty, value);
    //}

    //private static object CoerceSetters(BindableObject bindable, object value)
    //{
    //    if (!(value is ReadOnlyCollection<SkiaSetter> readonlyCollection))
    //    {
    //        return value;
    //    }

    //    return new ReadOnlyCollection<SkiaSetter>(
    //        readonlyCollection.Select(s => s)
    //            .ToList());
    //}

    //private static void SettersPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
    //{

    //    if (bindable is SkiaVisualState gradient)
    //    {
    //        if (oldvalue is INotifyCollectionChanged oldCollection)
    //        {
    //            oldCollection.CollectionChanged -= gradient.OnSkiaPropertySetterCollectionChanged;
    //        }
    //        if (newvalue is INotifyCollectionChanged newCollection)
    //        {
    //            newCollection.CollectionChanged += gradient.OnSkiaPropertySetterCollectionChanged;
    //        }

    //        gradient.Parent?.Update();
    //    }

    //}

    //private void OnSkiaPropertySetterCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //    switch (e.Action)
    //    {
    //        case NotifyCollectionChangedAction.Add:
    //            foreach (SkiaSetter newSkiaPropertySetter in e.NewItems)
    //            {
    //                _weakCollectionChangedSource.Raise(this, e);
    //            }

    //            break;

    //        case NotifyCollectionChangedAction.Reset:
    //        case NotifyCollectionChangedAction.Remove:
    //            foreach (SkiaSetter oldSkiaPropertySetter in e.OldItems ?? Array.Empty<SkiaSetter>())
    //            {
    //                _weakCollectionChangedSource.Raise(this, e);
    //            }

    //            break;
    //    }

    //    this.Parent?.Update();

    //}

    //#endregion


}

