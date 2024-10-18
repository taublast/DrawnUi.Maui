using AppoMobi.Maui.Gestures;
using DrawnUi.Maui.Infrastructure;
using DrawnUi.Maui.Internals;

namespace Sandbox.Views.Controls;

public enum PlayType
{
    Default,
    Next,
    Random
}

public record Transition(string Name, string Source, int Speed);

/// <summary>
/// Subclassed CarouselWithTransitions to add automation to transitions,
/// totally specific to this demo case
/// </summary>
public class AnimatedCarousel : CarouselWithTransitions
{

    public AnimatedCarousel()
    {
        //_shaders = Files.ListAssets(path); //not using this, will provide a limited list below:

        //some adapted shaders from https://github.com/gl-transitions/gl-transitions
        _transitions = new List<Transition>
        {
            //pointless
            //new("Bookflip", "bookflip.sksl", 1500),
            //new("Bounce", "bounce.sksl", 1500),

            new("Bow Tie Horizontal", "bowtiehorizontal.sksl", 750),
            new("Bow Tie Vertical", "bowtievertical.sksl", 750),
          
            //bugs
            //new("Butterfly Waves Crawler", "butterflywavescrawler.sksl", 250),
            
            new("Circlecrop", "circlecrop.sksl", 1500),

            new("Circleopen", "circleopen.sksl", 750),

            new("Colorphase", "colorphase.sksl", 750),

            new("Cross-hatch", "crosshatch.sksl", 1000),

            new("Cross-warp", "crosswarp.sksl", 750),

            new("Cross-zoom", "crosszoom.sksl", 750),

            //bugs
            new("Cube", "cube.sksl", 750),

            new("Doorway", "doorway.sksl", 750),

            new("Dreamy", "dreamy.sksl", 750),

            new("Dreamy Zoom", "dreamyzoom.sksl", 750),

            new("Edge", "edgetransition.sksl", 750),

            new("Fade", "fade.sksl", 500),

            new("Fade Color", "fadecolor.sksl", 750),

            new("Fade Grayscale", "fadegrayscale.sksl", 750),

            new("Film Burn", "filmburn.sksl", 1250),

            new("Fly Eye", "flyeye.sksl", 1000),

            new("Heart", "heart.sksl", 750),

            new("Kaleidoscope", "kaleidoscope.sksl", 1000),

            new("Morph", "morph.sksl", 500),

            new("Mosaic", "mosaic.sksl", 750),

            new("Page Curl", "pagecurl.sksl", 1000),

            new("Pixelize", "pixelize.sksl", 1000),

            new("Rolls", "rolls.sksl", 750),

            new("Scale In", "scalein.sksl", 750),

            new("Swirl", "swirl.sksl", 1250),

            new("Tangent Motion Blur", "tangentmotionblur.sksl", 1000),

            new("Tv Static", "tvstatic.sksl", 750),

            new("Waterdrop", "waterdrop.sksl", 750),

            new("Wind", "wind.sksl", 750),

            new("Window Blinds", "windowblinds.sksl", 750),

            new("Window Slice", "windowslice.sksl", 1000),

            new("Wipe Down", "wipedown.sksl", 750),

            new("Wipe Left", "wipeleft.sksl", 750),

            new("Wipe Right", "wiperight.sksl", 750),

            new("Wipe Up", "wipeup.sksl", 750)
        };

        SetTransition(_transitions.First(x => x.Name == "Page Curl"));
    }

    protected override void OnChildrenInitialized()
    {
        base.OnChildrenInitialized();

        SetupAnimator();
    }

    private RangeAnimator _animator;
    private LinearDirectionType animatingTo;

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
                    //every 
                    if (PlayingType == PlayType.Random)
                    {
                        SetTransition(GettRandomShader());
                    }
                    else
                    if (PlayingType == PlayType.Next)
                    {
                        SetTransition(GetNextShader());
                    }

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

                    SetupAnimator();
                }
            };

            _animator.Start((v) =>
            {

            }, 0, 1, (uint)AnimatorSpeedMs);
        }

    }

    public void SetTransition(Transition transition)
    {
        ShaderFile = transition.Source;
        LinearSpeedMs = transition.Speed;
    }

    public static readonly BindableProperty AnimatorSpeedMsProperty = BindableProperty.Create(nameof(AnimatorSpeedMsProperty),
        typeof(double),
        typeof(AnimatedCarousel),
        0.0,
        propertyChanged: (b, o, n) =>
        {
            if (b is AnimatedCarousel control)
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


    /// <summary>
    /// overrided to track selected image filename
    /// </summary>
    /// <param name="index"></param>
    protected override void OnSelectedIndexChanged(int index)
    {
        base.OnSelectedIndexChanged(index);

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

    private readonly List<Transition> _transitions;
    //private readonly List<string> _shaders;

    protected PlayType PlayingType { get; set; }

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

    public override ISkiaGestureListener OnSkiaGestureEvent(SkiaGesturesParameters args, GestureEventProcessingInfo apply)
    {

        if (args.Type == TouchActionResult.Tapped)
        {
            MainThread.BeginInvokeOnMainThread(SelectFIle);
            return this;
        }

        return base.OnSkiaGestureEvent(args, apply);
    }

    static List<SelectableAction> options;

    async void SelectFIle()
    {
        if (_transitions.Count > 1)
        {
            if (options == null)
            {
                options = _transitions.Select(x => new SelectableAction
                {
                    Action = async () =>
                    {
                        PlayingType = PlayType.Default;
                        SetTransition(x);
                    },
                    Title = x.Name
                }).ToList();
                options.Insert(0, new SelectableAction()
                {
                    Action = async () =>
                    {
                        PlayingType = PlayType.Random;
                        SetTransition(GettRandomShader());
                    },
                    Title = "Loop All Random"
                });
                options.Insert(0, new SelectableAction()
                {
                    Action = async () =>
                    {
                        PlayingType = PlayType.Next;
                        SetTransition(GetNextShader());
                    },
                    Title = "Loop All"
                });
            }

            var selected = await PresentSelection(options, "Select Shader") as SelectableAction;
            selected?.Action();
        }
    }

    private int _loopIndex;

    Transition GetNextShader()
    {
        _loopIndex++;
        if (_loopIndex > _transitions.Count - 1)
        {
            _loopIndex = 0;
        }
        return _transitions[_loopIndex];
    }

    Transition GettRandomShader()
    {
        var index = Random.Next(_transitions.Count - 1);
        return _transitions[index];
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
}