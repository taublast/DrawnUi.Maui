using Microsoft.Extensions.DependencyInjection.Extensions;
//using Microsoft.Extensions.Http;
//using Polly;
//using Polly.Timeout;
using System.Net;

namespace DrawnUi.Features.Images
{
    public static class ImagesExtensions
    {
        const string HttpClientKey = "drawnui";

        /*

        public static IServiceCollection AddUriImageSourceHttpClient(this IServiceCollection services,
            Action<HttpClient>? configureDelegate = null, Func<IHttpClientBuilder, IHttpClientBuilder>? delegateBuilder = null)
        {
            IHttpClientBuilder clientBuilder;

            if (configureDelegate != null)
            {
                clientBuilder = services.AddHttpClient(HttpClientKey, configureDelegate);
            }
            else
            {
                var retryPolicy = Policy
                    .HandleResult<HttpResponseMessage>(r =>
                        r.StatusCode == HttpStatusCode.GatewayTimeout
                        || r.StatusCode == HttpStatusCode.RequestTimeout)
                    .Or<HttpRequestException>()
                    .Or<TimeoutRejectedException>()
                    .WaitAndRetryAsync(new[]
                    {
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(3),
                    });

                clientBuilder = services.AddHttpClient(HttpClientKey, client =>
                    {
                        client.DefaultRequestHeaders.Add("User-Agent", Super.UserAgent);
                    })
                    .ConfigurePrimaryHttpMessageHandler(() =>
                    {
                        var handler = new HttpClientHandler();
                        if (handler.SupportsAutomaticDecompression)
                        {
                            handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
                        }

                        return handler;
                    })
                    .AddPolicyHandler(retryPolicy);
            }

            delegateBuilder?.Invoke(clientBuilder);

            //do not slow us down with logs spam
            //one could inject IHttpMessageHandlerBuilderFilter after this to enable logs back
            services.RemoveAll<IHttpMessageHandlerBuilderFilter>();

            return services;
        }

        */

        /// <summary>
        /// Will create a HttpClient with a UserAgent in headers defined by `Super.UserAgent`.
        /// You can define your own delegate to create HttpClient by setting Super.CreateHttpClient.
        /// Do not forget to dispose the client after usage, we do not use IHttpClientFactory by default.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static HttpClient? CreateHttpClient(this IServiceProvider services)
        {
            if (Super.CreateHttpClient != null) 
            {
                return Super.CreateHttpClient(services);
            }

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.ParseAdd(Super.UserAgent);
            return client;

            //return services.GetService<IHttpClientFactory>()?.CreateClient(HttpClientKey); :( we removed IHttpClientFactory for faster app startup
        }

    }
}
