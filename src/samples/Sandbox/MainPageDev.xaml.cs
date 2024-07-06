
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

public class BuggedLayout : SkiaLayout
{
    protected override void Draw(SkiaDrawingContext context, SKRect destination, float scale)
    {
        base.Draw(context, destination, scale);

        //if (UsingCacheType == SkiaCacheType.ImageDoubleBuffered && RenderObject != null)
        //{
        //    Debug.WriteLine($"[D] {Superview.CanvasView.CanvasSize.Width} -> {RenderObject.Bounds.Width} at {DrawingRect.Width}");

        //    if (DrawingRect.Width != RenderObject.Bounds.Width)
        //    {
        //        InvalidateMeasure();
        //    }
        //}

    }
}

public partial class MainPageDev : BasePage
{
    private readonly List<string> _shaders;

    public MainPageDev()
    {
        try
        {
            InitializeComponent();

            Test();

            _shaders = Files.ListAssets(path);
        }
        catch (Exception e)
        {
            Super.DisplayException(this, e);
        }
    }

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