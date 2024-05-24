using DrawnUi.Maui.Infrastructure.Models;

namespace Sandbox.Views.Xaml2Pdf;

public partial class ReportSample : SkiaLayout
{
    public ReportSample()
    {
        InitializeComponent();
    }

    private bool _AllImagesLoaded;
    public bool AllImagesLoaded
    {
        get
        {
            return _AllImagesLoaded;
        }
        set
        {
            if (_AllImagesLoaded != value)
            {
                _AllImagesLoaded = value;
                OnPropertyChanged();
            }
        }
    }

    private void SkiaImage_OnOnSuccess(object sender, ContentLoadedEventArgs e)
    {
        AllImagesLoaded = true;
    }
}