using BruTile;
using BruTile.Cache;
using BruTile.Predefined;
using BruTile.Web;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Rendering;
using Mapsui.Rendering.Skia;
using Mapsui.Rendering.Skia.SkiaStyles;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.Tiling.Fetcher;
using Mapsui.Tiling.Layers;
using Mapsui.Tiling.Rendering;
using Mapsui.Widgets.ButtonWidgets;
using Mapsui.Widgets.InfoWidgets;


namespace Sandbox.Views
{
    public partial class MainPageMapsUi : ContentPage
    {

        public MainPageMapsUi()
        {
            try
            {
                InitializeComponent();

                //avoid setting context BEFORE InitializeComponent, can bug 
                //having parent BindingContext still null when constructing from xaml
                BindingContext = new MainPageViewModel();
            }
            catch (Exception e)
            {
                Super.DisplayException(this, e);
                Console.WriteLine(e);
            }
        }

        bool once;

        /*

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!once)
            {
                once = true;

                var map = new Mapsui.Map()
                {
                    CRS = "EPSG:3857",
                    BackColor = Mapsui.Styles.Color.Transparent
                    //Transformation = new MinimalTransformation()
                };

                map.CRS = "EPSG:3857";

                var _layer = OpenCustomStreetMap.CreateTileLayer("raceboxcompanionapp-mobile");

                map.Layers.Add(_layer);
                map.Widgets.Clear();
                map.Widgets.Add(new Mapsui.Widgets.ScaleBar.ScaleBarWidget(mapControl.Map)
                {
                    TextAlignment = Mapsui.Widgets.Alignment.Center,
                    HorizontalAlignment = Mapsui.Widgets.HorizontalAlignment.Left,
                    VerticalAlignment = Mapsui.Widgets.VerticalAlignment.Bottom,
                });

                //adds the +/- zoom widget
                map.Widgets.Add(new ZoomInOutWidget
                {
                    Margin = new(10, 20),
                });

                mapControl.Map = map;
                //48.854985, 2.311670
                mapControl.WasFirstTimeDrawn += (s, a) =>
                {
                    var point = new MPoint(48.856663, 2.351556);
                    var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(point.X, point.Y).ToMPoint();

                    mapControl.Map.Navigator.CenterOnAndZoomTo(
                        sphericalMercatorCoordinate, mapControl.Map.Navigator.Resolutions[2]);
                };

                mapControl?.Refresh();

            }

        }

        */

        public static Mapsui.Map CreateMap()
        {
            var map = new Mapsui.Map();

            map.Layers.Add(OpenStreetMap.CreateTileLayer());
            //map.Layers.Add(CreateStylesLayer(map.Extent));

            map.Widgets.Add(new MapInfoWidget(map));

            return map;
        }

        public class TileRenderer : ISkiaStyleRenderer
        {
            public bool Draw(SKCanvas canvas, Viewport viewport, ILayer layer, IFeature feature, IStyle style, IRenderCache renderCache,
                long iteration)
            {



                return false;
            }
        }

        public static class OpenCustomStreetMap
        {
            public class CustomizedLayer : TileLayer
            {
                public CustomizedLayer(ITileSource tileSource, int minTiles = 200, int maxTiles = 300, IDataFetchStrategy dataFetchStrategy = null, IRenderFetchStrategy renderFetchStrategy = null, int minExtraTiles = -1, int maxExtraTiles = -1, Func<TileInfo, Task<IFeature>> fetchTileAsFeature = null) : base(tileSource, minTiles, maxTiles, dataFetchStrategy, renderFetchStrategy, minExtraTiles, maxExtraTiles, fetchTileAsFeature)
                {
                }

            }

            public static IPersistentCache<byte[]>? DefaultCache = null;

            private static readonly BruTile.Attribution OpenStreetMapAttribution = new(
                "OpenStreetMap", "https://www.openstreetmap.org/copyright");

            public static TileLayer CreateTileLayer(string? userAgent = null)
            {
                userAgent ??= $"user-agent-of-{Path.GetFileNameWithoutExtension(System.AppDomain.CurrentDomain.FriendlyName)}";

                return new CustomizedLayer(CreateTileSource(userAgent)) { Name = "OpenStreetMap" };
            }

            private static HttpTileSource CreateTileSource(string userAgent)
            {

                return new HttpTileSource(new GlobalSphericalMercator(),
                    "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
                    new[] { "a", "b", "c" }, name: "OpenStreetMap",
                    attribution: OpenStreetMapAttribution, userAgent: userAgent, persistentCache: DefaultCache);
            }
        }


    }
}