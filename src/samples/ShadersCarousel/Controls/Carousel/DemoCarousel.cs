using DrawnUi.Maui.Draw;
using DrawnUi.Maui.Internals;
using System.Windows.Input;

namespace ShadersCarouselDemo.Controls.Carousel;

/// <summary>
/// Subclassed CarouselWithTransitions to add automation to transitions,
/// totally specific to this demo case
/// </summary>
public class DemoCarousel : ShadersCarousel
{
    public override void OnParentChanged(IDrawnBase newvalue, IDrawnBase oldvalue)
    {
        base.OnParentChanged(newvalue, oldvalue);

        if (Transitions == null)
            SetupSources();
    }

    //public override ISkiaGestureListener OnSkiaGestureEvent(SkiaGesturesParameters args, GestureEventProcessingInfo apply)
    //{

    //    if (args.Type == TouchActionResult.Tapped)
    //    {
    //        MainThread.BeginInvokeOnMainThread(SelectFIle);
    //        return this;
    //    }

    //    return base.OnSkiaGestureEvent(args, apply);
    //}

    /// <summary>
    /// some adapted shaders from https://github.com/gl-transitions/gl-transitions
    /// </summary>
    public virtual void SetupSources()
    {
        Transitions = new List<Transition>
        {
            new("Bow Tie Vertical", "bowtievertical.sksl", 750),
            new("Butterfly Waves Crawler", "butterflywavescrawler.sksl", 1500),
            new("Circle Crop", "circlecrop.sksl", 1500),
            new("Circle Open", "circleopen.sksl", 750),
            new("Color Phase", "colorphase.sksl", 750),
            new("Cross-warp", "crosswarp.sksl", 750),
            new("Cube", "cube.sksl", 750),
            new("Doorway", "doorway.sksl", 750),
            new("Dreamy", "dreamy.sksl", 750),
            new("Edge", "edgetransition.sksl", 750),
            new("Fade Color", "fadecolor.sksl", 750),
            new("Fade Grayscale", "fadegrayscale.sksl", 750),
            new("Fly Eye", "flyeye.sksl", 1000),
            new("Heart", "heart.sksl", 750),
            new("Kaleidoscope", "kaleidoscope.sksl", 1000),
            new("Morph", "morph.sksl", 500),
            new("Page Curl", "pagecurlbtm.sksl", 1000),
            new("Page Curl Top", "pagecurl.sksl", 1000),
            new("Pixelize", "pixelize.sksl", 1000),
            new("Radial", "radial.sksl", 750),
            new("Rectangle Crop", "rectanglecrop.sksl", 750),
            new("Scale In", "scalein.sksl", 750),
            new("Squeeze Wire", "squeezewire.sksl", 1250),
            new("Swap", "swap.sksl", 1250),
            new("Swirl", "swirl.sksl", 1250),
            new("Tv Static", "tvstatic.sksl", 750),
            new("Waterdrop", "waterdrop.sksl", 750),
            new("Wind", "wind.sksl", 750),
            new("Window Slice", "windowslice.sksl", 1000),
            new("Wipe Left", "wipeleft.sksl", 750),
        };

        SetTransition(Transitions.First(x => x.Name == "Wind"));
    }

    protected override void OnChildrenInitialized()
    {
        base.OnChildrenInitialized();

        SetupAnimator();
    }

    private RangeAnimator _animator;
    private LinearDirectionType animatingTo;


    protected override void OnTransitionChanged()
    {
        base.OnTransitionChanged();

        if (!InTransition)
        {
            if (PlayingType == PlayType.Random)
            {
                SetTransition(GetRandomShader());
            }
            else
            if (PlayingType == PlayType.Next)
            {
                SetTransition(GetNextShader());
            }
        }
    }

    public void PlayOne()
    {
        //ping-pong-looping SelectedIndex
        var index = SelectedIndex;
        if (animatingTo == LinearDirectionType.Forward)
        {
            index++;
            if (index > MaxIndex)
            {
                animatingTo = LinearDirectionType.Backward;
                index -= 2;
            }
        }
        else
        {
            index--;
            if (index < 0)
            {
                animatingTo = LinearDirectionType.Forward;
                index = 1;
            }
        }
        SelectedIndex = index;
    }

    void SetupAnimator()
    {
        if (!ChildrenInitialized)
            return;

        if (_animator != null)
        {
            _animator.Stop();
            _animator.Dispose();
            _animator = null;
        }

        if (_animator == null && AnimatorSpeedMs > 0)
        {
            _animator = new(this)
            {
                CycleFInished = () =>
                {
                    PlayOne();

                    SetupAnimator();
                }
            };

            _animator.Start((v) =>
            {

            }, 0, 1, (uint)AnimatorSpeedMs);
        }

    }

    public override void OnDisposing()
    {
        base.OnDisposing();

        _transition = null;
    }

    private Transition _transition = null;

    public void SetTransition(Transition transition)
    {
        _transition = transition;
        ShaderName = transition.Name;
        ShaderFile = transition.Source;
        LinearSpeedMs = transition.Speed * SpeedRatio;
    }

    public void ApplySpeedRatio()
    {
        if (_transition != null)
        {
            LinearSpeedMs = _transition.Speed * SpeedRatio;
        }
    }





    /// <summary>
    /// overrided to track selected image filename
    /// </summary>
    /// <param name="index"></param>
    protected override void OnSelectedIndexChanged(int index)
    {
        base.OnSelectedIndexChanged(index);

        //Debug.WriteLine($"SelectedIndex {SelectedIndex}");

        if (ItemsSource is IList<string> strings)
        {
            if (index >= 0)
            {
                SelectedString = strings[index];
            }
            else
            {
                SelectedString = string.Empty;
            }
        }
    }

    private string _selectedString;
    /// <summary>
    /// to track selected image filename
    /// </summary>
    public string SelectedString
    {
        get
        {
            return _selectedString;
        }
        set
        {
            if (_selectedString != value)
            {
                _selectedString = value;
                OnPropertyChanged();
            }
        }
    }



    #region Select transition

    protected List<Transition> Transitions;
    protected List<SelectableAction> SelectOptions;
    protected int LoopIndex;

    private string path = @"Shaders\transitions";
    public string FullShaderPath
    {
        get
        {
            return $"{path}\\{ShaderFile}";
        }
        set
        {

        }
    }

    private string _ShaderName;
    public string ShaderName
    {
        get
        {
            return _ShaderName;
        }
        set
        {
            if (_ShaderName != value)
            {
                _ShaderName = value;
                OnPropertyChanged();
            }
        }
    }

    private string _ShaderFile;
    public string ShaderFile
    {
        get
        {
            return _ShaderFile;
        }
        set
        {
            if (_ShaderFile != value)
            {
                _ShaderFile = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FullShaderPath));
                ShaderFilename = FullShaderPath;
            }
        }
    }

    public ICommand CommandSelectShader
    {
        get
        {
            return new Command((ctx) =>
            {
                MainThread.BeginInvokeOnMainThread(SelectFIle);
            });
        }
    }

    protected virtual List<SelectableAction> CreateSelectList()
    {
        var options = Transitions.Select(x => new SelectableAction
        {
            Action = async () =>
            {
                PlayingType = PlayType.Default;
                SetTransition(x);
            },
            Title = x.Name
        }).ToList();

        return options;
    }

    protected virtual async void SelectFIle()
    {
        if (Transitions.Count > 1)
        {
            if (SelectOptions == null)
            {
                SelectOptions = CreateSelectList();
            }

            var selected = await PresentSelection(SelectOptions, "Select Shader") as SelectableAction;
            selected?.Action();
        }
    }

    protected virtual Transition GetNextShader()
    {
        LoopIndex++;
        if (LoopIndex > Transitions.Count - 1)
        {
            LoopIndex = 0;
        }
        return Transitions[LoopIndex];
    }

    protected virtual Transition GetRandomShader()
    {
        var index = Random.Next(Transitions.Count - 1);
        return Transitions[index];
    }

    public async Task<ISelectableOption> PresentSelection(IEnumerable<ISelectableOption> options,
        string title = null, string cancel = null)
    {
        if (string.IsNullOrEmpty(title))
            title = "Select";

        if (string.IsNullOrEmpty(cancel))
            cancel = "Cancel";

        var result = await App.Current.MainPage.DisplayActionSheet(title, cancel,
            null, options.Select(x => x.Title).ToArray()
        );

        if (string.IsNullOrEmpty(result))
        {
            return null; //cancel
        }

        var selected = options.FirstOrDefault(x => x.Title == result);
        return selected;
    }

    #endregion

    public static readonly BindableProperty AnimatorSpeedMsProperty = BindableProperty.Create(nameof(AnimatorSpeedMsProperty),
        typeof(double),
        typeof(DemoCarousel),
        0.0,
        propertyChanged: (b, o, n) =>
        {
            if (b is DemoCarousel control)
            {
                control.SetupAnimator();
            }
        });

    /// <summary>
    /// If you set this higher than 0 will have an animator running ping-pong through slides.
    /// </summary>
    public double AnimatorSpeedMs
    {
        get { return (double)GetValue(AnimatorSpeedMsProperty); }
        set { SetValue(AnimatorSpeedMsProperty, value); }
    }


    public static readonly BindableProperty SpeedRatioProperty = BindableProperty.Create(
        nameof(SpeedRatio),
        typeof(double),
        typeof(DemoCarousel),
        1.0,
        propertyChanged: (b, o, n) =>
        {
            if (b is DemoCarousel control)
            {
                control.ApplySpeedRatio();
            }
        });


    public double SpeedRatio
    {
        get { return (double)GetValue(SpeedRatioProperty); }
        set { SetValue(SpeedRatioProperty, value); }
    }

    public static readonly BindableProperty PlayingTypeProperty = BindableProperty.Create(nameof(PlayingType),
    typeof(PlayType),
    typeof(DemoCarousel),
    PlayType.Default);
    public PlayType PlayingType
    {
        get { return (PlayType)GetValue(PlayingTypeProperty); }
        set { SetValue(PlayingTypeProperty, value); }
    }

}