using DrawnUi.Draw;
using DrawnUi.Features.Images;
using NSubstitute;
using Xunit;

namespace UnitTests;

public class SkiaImageManagerTests : DrawnTestsBase
{

    void SetupApplicationWithHttpClient(Action<IServiceCollection> services = null)
    {
        var builder = MauiApp.CreateBuilder(useDefaults: false);
        //builder.Services.AddUriImageSourceHttpClient();
        services?.Invoke(builder.Services);
        var mauiApp = builder
            .Build();

        var fakeMauiContext = Substitute.For<IMauiContext>();
        var fakeHandler = Substitute.For<IElementHandler>();
        fakeMauiContext.Services.Returns(mauiApp.Services);
        fakeHandler.MauiContext.Returns(fakeMauiContext);
        var app = new Application();
        Application.Current = app;
        app.Handler = fakeHandler;
    }

    HttpClient CreateReusableClient()
    {
        SetupApplicationWithHttpClient();

        var client = Super.Services.CreateHttpClient();

        return client;
    }

    [Fact]
    public void ShouldCreateHttpClient()
    {
        var client = CreateReusableClient();

        Assert.NotNull(client);
    }


    [Fact]
    public async Task LoadImageFromInternet()
    {
        SetupApplicationWithHttpClient();

        var source = new UriImageSource
        {
            Uri = new Uri("https://upload.wikimedia.org/wikipedia/commons/1/12/Wikipedia.png"),
        };

        var bitmap = await SkiaImageManager.LoadImageFromInternetAsync(source, CancellationToken.None);

        Assert.Equal(135, bitmap.Width);
    }

    [Fact]
    public async Task LoadImageFromInternetWIthCustomizedClient()
    {
        Super.CreateHttpClient = (services =>
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Tests");
            return client;
        });

        var source = new UriImageSource
        {
            Uri = new Uri("https://www.mediawiki.org/w/index.php?title=Special:Redirect/file/Wikipedia.png"),
        };

        var bitmap = await SkiaImageManager.LoadImageFromInternetAsync(source, CancellationToken.None);

        Assert.Equal(135, bitmap.Width);
    }

    [Fact]
    public async Task LoadImageFromInternetRequiresUserAgent()
    {
        SetupApplicationWithHttpClient();

        var source = new UriImageSource
        {
            Uri = new Uri("https://www.mediawiki.org/w/index.php?title=Special:Redirect/file/Wikipedia.png"),
        };

        var bitmap = await SkiaImageManager.LoadImageFromInternetAsync(source, CancellationToken.None);

        Assert.Equal(135, bitmap.Width);
    }


}
