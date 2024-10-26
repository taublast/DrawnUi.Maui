
using AppoMobi.Specials;
using DrawnUi.Maui.Infrastructure;
using DrawnUi.Maui.Internals;
using Sandbox;
using Sandbox.Views;



namespace MauiNet8;



public partial class MainPageDev : BasePage
{
    private readonly List<string> _shaders;

    public MainPageDev()
    {
        try
        {

            Items.AddRange(new[]
            {
                "8.jpg","monkey1.jpg","hugrobot2.jpg"
            });

            InitializeComponent();

            _shaders = Files.ListAssets(path);

            BindingContext = this;
        }
        catch (Exception e)
        {
            Super.DisplayException(this, e);
        }
    }

    public ObservableRangeCollection<string> Items { get; } = new();

    private int _SelectedIndex;
    public int SelectedIndex
    {
        get
        {
            return _SelectedIndex;
        }
        set
        {
            if (_SelectedIndex != value)
            {
                _SelectedIndex = value;
                OnPropertyChanged();
            }
        }
    }


    void Test()
    {
        // string shaderCode = SkSl.LoadFromResources($"{MauiProgram.ShadersFolder}/apple.sksl");
        //var effect = SkSl.Compile(shaderCode);
    }

    private double _SpeedMs = 1750;
    public double SpeedMs
    {
        get
        {
            return _SpeedMs;
        }
        set
        {
            if (_SpeedMs != value)
            {
                _SpeedMs = value;
                OnPropertyChanged();
                MainCarousel.LinearSpeedMs = SpeedMs;
            }
        }
    }


    private bool _AutoPlay;
    public bool AutoPlay
    {
        get
        {
            return _AutoPlay;
        }
        set
        {
            if (_AutoPlay != value)
            {
                _AutoPlay = value;
                OnPropertyChanged();
                if (value)
                {
                    MainCarousel.AnimatorSpeedMs = SpeedMs;
                }
                else
                {
                    MainCarousel.AnimatorSpeedMs = 0;
                }
            }
        }
    }


    async void SelectFIle()
    {
        if (_shaders.Count > 1)
        {
            var options = _shaders.Select(name => new SelectableAction
            {
                Action = async () =>
                {
                    ShaderFile = name;
                },
                Title = name
            }).ToList();
            var selected = await PresentSelection(options, "Select Shader") as SelectableAction;
            selected?.Action();
        }
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

    private void SkiaButton_OnTapped(object sender, SkiaGesturesParameters e)
    {
        MainThread.BeginInvokeOnMainThread(SelectFIle);
    }

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

    private string _ShaderFile = "dreamy.sksl";
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
            }
        }
    }

}