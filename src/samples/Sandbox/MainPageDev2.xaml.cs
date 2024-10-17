
using AppoMobi.Specials;
using DrawnUi.Maui.Infrastructure;
using DrawnUi.Maui.Internals;
using Sandbox;
using Sandbox.Resources.Strings;
using Sandbox.Views;
using Sandbox.Views.Xaml2Pdf;
using System.Diagnostics;
using System.Text;



namespace MauiNet8;



public partial class MainPageDev2 : BasePage
{
    private readonly List<string> _shaders;

    public MainPageDev2()
    {
        try
        {

            Items.AddRange(new[]
            {
                "8.jpg","monkey1.jpg","hugrobot2.jpg"
            });

            InitializeComponent();

            _shaders = Files.ListAssets(path);


        }
        catch (Exception e)
        {
            Super.DisplayException(this, e);
        }
    }

    public ObservableRangeCollection<string> Items { get; } = new();

    void Test()
    {
        // string shaderCode = SkSl.LoadFromResources($"{MauiProgram.ShadersFolder}/apple.sksl");
        //var effect = SkSl.Compile(shaderCode);
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