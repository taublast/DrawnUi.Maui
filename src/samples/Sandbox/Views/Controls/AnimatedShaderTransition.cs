using AppoMobi.Maui.Gestures;
using DrawnUi.Maui.Infrastructure;
using DrawnUi.Maui.Internals;

namespace Sandbox.Views.Controls;

/// <summary>
/// Custom control to ping-pong loop transition and change to new random file after every animation cycle
/// </summary>
public class AnimatedShaderTransition : ShaderTransition, ISkiaGestureListener
{
    public AnimatedShaderTransition()
    {
        _shaders = Files.ListAssets(path);

        ShaderFile = "fade.sksl"; //default
    }

    private readonly List<string> _shaders;

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
    private PingPongAnimator _animator;

    protected override void OnLayoutReady()
    {
        base.OnLayoutReady();

        SetupAnimator();
    }

    void SetupAnimator()
    {
        if (!IsLayoutReady)
            return;

        if (_animator != null)
        {
            _animator.Stop();
            _animator.Dispose();
            _animator = null;
        }

        if (_animator == null)
        {
            _animator = new(this)
            {
                OnCycleFInished = () =>
                {
                    if (PlayingType == PlayType.Random)
                    {
                        ShaderFile = GettRandomShader();
                    }
                    else
                    if (PlayingType == PlayType.Next)
                    {
                        ShaderFile = GetNextShader();
                    }
                }
            };

            _animator.Start((v) =>
            {
                this.Progress = v;
                Update();
            }, 0, 1, (uint)DurationMs);
        }

    }


    public static readonly BindableProperty DurationMsProperty = BindableProperty.Create(nameof(DurationMsProperty),
        typeof(double),
        typeof(ShaderAnimatedTransitionEffect),
        3500.0,
        propertyChanged: (b, o, n) =>
        {
            if (b is AnimatedShaderTransition control)
            {
                control.SetupAnimator();
            }
        });



    public double DurationMs
    {
        get { return (double)GetValue(DurationMsProperty); }
        set { SetValue(DurationMsProperty, value); }
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

    public bool OnFocusChanged(bool focus)
    {
        return false;
    }

    static List<SelectableAction> options;

    async void SelectFIle()
    {
        if (_shaders.Count > 1)
        {
            if (options == null)
            {
                options = _shaders.Select(name => new SelectableAction
                {
                    Action = async () =>
                    {
                        PlayingType = PlayType.Default;
                        ShaderFile = name;
                    },
                    Title = name
                }).ToList();
                options.Insert(0, new SelectableAction()
                {
                    Action = async () =>
                    {
                        PlayingType = PlayType.Random;
                        ShaderFile = GettRandomShader();
                    },
                    Title = "Loop All Random"
                });
                options.Insert(0, new SelectableAction()
                {
                    Action = async () =>
                    {
                        PlayingType = PlayType.Next;
                        ShaderFile = GetNextShader();
                    },
                    Title = "Loop All"
                });
            }

            var selected = await PresentSelection(options, "Select Shader") as SelectableAction;
            selected?.Action();
        }
    }

    private int _loopIndex;

    string GetNextShader()
    {
        _loopIndex++;
        if (_loopIndex > _shaders.Count - 1)
        {
            _loopIndex = 0;
        }
        return _shaders[_loopIndex];
    }

    string GettRandomShader()
    {
        var index = Random.Next(_shaders.Count - 1);
        return _shaders[index];
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

}