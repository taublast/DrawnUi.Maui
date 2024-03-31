using Mapsui;
using Mapsui.ArcGIS.DynamicProvider;
using Mapsui.Extensions;
using Mapsui.Projections;
using Mapsui.Samples.Maui;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.Widgets.ButtonWidgets;

namespace Sandbox.Views
{
    public partial class MainPageMap : ContentPage
    {

        public MainPageMap()
        {
            try
            {
                InitializeComponent();

                //avoid setting context BEFORE initialize component, can bug 
                //having parent context still null when constructing from xaml

                BindingContext = new MainPageViewModel();
            }
            catch (Exception e)
            {
                Super.DisplayException(this, e);
                Console.WriteLine(e);
            }
        }

        bool once;



        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!once)
            {
                once = true;

                var map = mapControl.Map;

                //var map = new Mapsui.Map()
                //{
                //    CRS = "EPSG:3857",
                //    BackColor = Mapsui.Styles.Color.Transparent
                //    //Transformation = new MinimalTransformation()
                //};

                map.CRS = "EPSG:3857";

                mapControl.Renderer.StyleRenderers.TryGetValue(typeof(RasterStyle), out var existing);

                mapControl.Renderer.StyleRenderers[typeof(RasterStyle)] = new DrawnUiRasterStyleRenderer();

                var _layer = MainPageMapsUi.OpenCustomStreetMap.CreateTileLayer("drawnui-sandbox");

                map.Layers.Clear();
                map.Layers.Add(_layer);

                map.Widgets.Clear();
                map.Widgets.Add(new Mapsui.Widgets.ScaleBar.ScaleBarWidget(mapControl.Map)
                {
                    TextAlignment = Mapsui.Widgets.Alignment.Center,
                    HorizontalAlignment = Mapsui.Widgets.HorizontalAlignment.Center,
                    VerticalAlignment = Mapsui.Widgets.VerticalAlignment.Bottom,
                    Margin = new(16, 40),
                });

                //adds the +/- zoom widget
                map.Widgets.Add(new ZoomInOutWidget
                {
                    Margin = new(16, 40),
                });

                //mapControl.Map = map;
                //48.854985, 2.311670
                //mapControl.WasFirstTimeDrawn += (s, a) =>
                //{
                //    var point = new MPoint(48.856663, 2.351556);
                //    var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(point.X, point.Y).ToMPoint();

                //    mapControl.Map.Navigator.CenterOnAndZoomTo(
                //        sphericalMercatorCoordinate, mapControl.Map.Navigator.Resolutions[2]);
                //};

                mapControl?.Refresh();

                var point = new MPoint(48.856663, 2.351556);
                var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(point.X, point.Y).ToMPoint();

                mapControl.Map.Navigator.CenterOnAndZoomTo(
                    sphericalMercatorCoordinate, mapControl.Map.Navigator.Resolutions[2]);

            }

        }


    }
}