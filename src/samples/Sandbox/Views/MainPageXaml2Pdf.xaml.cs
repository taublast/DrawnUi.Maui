using DrawnUi.Maui.Infrastructure;
using Sandbox.Views.Xaml2Pdf;

namespace Sandbox.Views
{
    public partial class MainPageXaml2Pdf : BasePage
    {

        public MainPageXaml2Pdf()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception e)
            {
                Super.DisplayException(this, e);
            }
        }

        string GenerateFileName(DateTime timeStamp, string extension)
        {
            var filename = $"DrawnUi_{timeStamp:yyyy-MM-dd_HHmmss}.{extension}";

            return filename;
        }

        private bool _lockLogs;

        private string _BindableText;
        public string BindableText
        {
            get
            {
                return _BindableText;
            }
            set
            {
                if (_BindableText != value)
                {
                    _BindableText = value;
                    OnPropertyChanged();
                }
            }
        }

        private void SkiaButton_OnTapped(object sender, SkiaGesturesParameters e)
        {
            CreatePdf(1240);  // A4 page for 150 DPI
        }

        private void SkiaButton_OnTapped2(object sender, SkiaGesturesParameters e)
        {
            CreatePdf(620);  // A6 page for 150 DPI
        }

        async Task CreatePdf(float width)
        {
            //setup our report to print
            BindableText = "This text came from bindings";
            var vendor = "DrawnUI";
            var filename = GenerateFileName(DateTime.Now, "pdf");

            var layout = new ReportSample()
            {
                BindingContext = this //whatever you want, you can have bindings inside your report
            };

            //render and share
            Files.CheckPermissionsAsync(async () =>
             {

                 try
                 {
                     _lockLogs = true;
                     string fullFilename = null;
                     var subfolder = "Pdf";
                     var scale = 1; //do not change this
                     var destination = new SKRect(0, 0, width, float.PositiveInfinity);
                     var measured = layout.Measure(destination.Width, destination.Height, scale);

                     //prepare DrawingRect
                     layout.Arrange(new SKRect(0, 0, layout.MeasuredSize.Pixels.Width, layout.MeasuredSize.Pixels.Height),
                         layout.MeasuredSize.Pixels.Width, layout.MeasuredSize.Pixels.Height, scale);

                     var reportSize = new SKSize(measured.Units.Width, measured.Units.Height);

                     //we need a local file to ba saved in order to share it
                     fullFilename = Files.GetFullFilename(filename, StorageType.Cache, subfolder);

                     if (File.Exists(fullFilename))
                     {
                         File.Delete(fullFilename);
                     }

                     var area = new SKRect(layout.DrawingRect.Left, layout.DrawingRect.Top, reportSize.Width, reportSize.Height);

                     using (var ms = new MemoryStream())
                     using (var stream = new SKManagedWStream(ms))
                     {
                         using (var document = SKDocument.CreatePdf(stream, new SKDocumentPdfMetadata
                         {
                             Author = vendor,
                             Producer = vendor,
                             Subject = this.Title
                         }))
                         {
                             using (var canvas = document.BeginPage(reportSize.Width, reportSize.Height))
                             {
                                 var ctx = new SkiaDrawingContext()
                                 {
                                     Canvas = canvas,
                                     Width = reportSize.Width,
                                     Height = reportSize.Height
                                 };

                                 //with no async stuff this is enough for most cases
                                 layout.Render(ctx, new SKRect(0, 0, reportSize.Width, reportSize.Height), scale);

                                 //in our specific case we have images inside that load async,
                                 //so wait for them and render final result
                                 while (!layout.AllImagesLoaded)
                                 {
                                     await Task.Delay(50);
                                 }
                                 layout.Render(ctx, new SKRect(0, 0, reportSize.Width, reportSize.Height), scale);

                             }
                             document.EndPage();
                             document.Close();
                         }

                         ms.Position = 0;
                         var content = ms.ToArray();

                         var file = Files.OpenFile(fullFilename, StorageType.Cache, subfolder);

                         // Write the bytes to the FileStream of the FileDescriptor
                         await file.Handler.WriteAsync(content, 0, content.Length);

                         // Ensure all bytes are written to the underlying device
                         await file.Handler.FlushAsync();

                         Files.CloseFile(file, true);
                         await Task.Delay(500);
                     }

                     //can share the file now
                     Files.Share("PDF", new string[] { fullFilename });
                 }
                 catch (Exception e)
                 {
                     Super.Log(e);
                 }
                 finally
                 {
                     _lockLogs = false;
                 }
             });
        }

    }
}