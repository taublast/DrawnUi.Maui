using AppoMobi.Maui.Gestures;
using DrawnUi.Maui.Infrastructure;

namespace Sandbox.Views
{
    public partial class MainPageShader : ContentPage
    {

        private void OnTappedDebug(object sender, TouchActionEventArgs e)
        {

            MainCanvas.PostponeExecutionBeforeDraw(() =>
            {



                //var control = new SkiaShape
                //{
                //    BackgroundColor = Colors.Red,
                //    WidthRequest = 100,
                //    HeightRequest = 20
                //};
                //StackContainer.AddSubView(control);

                //return;
                ////var check = this.GetVisualElementWindow();
                //var check1 = StackOptions.GetVisualTreeDescendants();
                //if (check1.Count > 0)
                //{
                //    foreach (var visualTreeElement in check1)
                //    {
                //        Trace.WriteLine($"{visualTreeElement}");
                //    }
                //}
            });
        }

        /*
         
           fragColor = iImage1.eval(coords);  =>  sample(iImage1, coords);

         */





        void DoTest()
        {

            // using Skia's SkSL
            string shaderCode = @"

                 uniform float iTime;  // Manually updated time variable
                  uniform float2 iResolution; // Screen iResolution
                  uniform shader iImage1; // Texture

                  const float PI = 6.28318530718;
                  const float DYNAMIC_OFFSET_Y = 0.46;
                  const float MOD_TIME_LIMIT = 3.0;
                  const float EXPLOSION_LIGHT_INTENSITY = 0.16;
                  const float EXPLOSION_LIGHT_2_INTENSITY = 0.14;
                  const float EXPLOSION_LIGHT_SHIFT = 8.0;
                  const float EXPLOSION_LIGHT_2_SHIFT = 8.0;
                  const float EXPLOSION_LIGHT_Y_OFFSET = 0.4;
                  const float EXPLOSION_LIGHT_2_Y_OFFSET = 0.4;
                  const float NUM_DIRS = 60.0;
                  const float QUALITY = 10.0;
                  const float BLUR_RAD_MULTIPLIER = 0.5;
                  const float BLUR_RAD_LIGHT_MULTIPLIER = 0.05;

                 float smoothstep2(float a, float b, float x) {
                        float t = clamp((x - a) / (b - a), 0.0, 1.0);
                        return t * t * (3.0 - 2.0 * t);
                    }

                  half4 main(float2 fragCoord) {
                      float limitedTime = iTime - MOD_TIME_LIMIT * floor(iTime / MOD_TIME_LIMIT);
                      float timeSq = limitedTime * limitedTime;
                      float timeCu = limitedTime * timeSq;

                    float2 normalizedUv = fragCoord / iResolution;
                    float2 distortedUv = float2(
                        normalizedUv.x + (normalizedUv.x - 0.5) * pow(normalizedUv.y, 6.0) * timeCu * 0.1,
                        (normalizedUv.y * (normalizedUv.y * pow((1.0 - timeSq * 0.01), 8.0)) + (1.0 - normalizedUv.y) * normalizedUv.y)
                    );
                    distortedUv = mix(normalizedUv, distortedUv, smoothstep2(1.1, 1.0, limitedTime));

                    half4 color = sample(iImage1, distortedUv);
                    float2 explosionShift = float2(0.0);
                    float explosionLight = 0.0;

                    if (limitedTime >= 1.0) {
                        float adjustedTime = limitedTime - 1.0;
                        normalizedUv -= 0.5;
                        normalizedUv.x *= iResolution.x / iResolution.y;
                        normalizedUv.x -= 0.1;

                        float2 uvExplode = float2(normalizedUv.x + 0.1, normalizedUv.y + DYNAMIC_OFFSET_Y);
                        explosionLight = (adjustedTime * EXPLOSION_LIGHT_INTENSITY) / length(uvExplode);
                        explosionLight = smoothstep2(0.09, 0.05, explosionLight) *
                                         smoothstep2(0.04, 0.07, explosionLight) *
                                         (normalizedUv.y + 0.05);
                        explosionShift = float2(-EXPLOSION_LIGHT_SHIFT * explosionLight * normalizedUv.x,
                                                -4.0 * explosionLight * (normalizedUv.y - EXPLOSION_LIGHT_Y_OFFSET)) * 0.1;

                        float explosionLight2 = ((adjustedTime - 0.085) * EXPLOSION_LIGHT_2_INTENSITY) / length(uvExplode);
                        explosionLight2 = smoothstep2(0.09, 0.05, explosionLight2) *
                                          smoothstep2(0.04, 0.07, explosionLight2) *
                                          (normalizedUv.y + 0.05);
                        explosionShift += float2(-EXPLOSION_LIGHT_2_SHIFT * explosionLight2 * normalizedUv.x,
                                                 -4.0 * explosionLight2 * (normalizedUv.y - EXPLOSION_LIGHT_2_Y_OFFSET)) * -0.02;
                    }

                    float2 finalUv = distortedUv + explosionShift;
                    color = sample(iImage1, finalUv);
                    color += explosionLight * 500.0 * smoothstep2(1.05, 1.1, limitedTime);

                    float blurRad = timeSq * 0.1 * pow(normalizedUv.y, 6.0) * BLUR_RAD_MULTIPLIER;
                    blurRad *= smoothstep2(1.3, 0.9, limitedTime);
                    blurRad += explosionLight * BLUR_RAD_LIGHT_MULTIPLIER;

                    for(float angle = 0.0; angle < PI; angle += PI / NUM_DIRS) {
                        for(float i = 1.0 / QUALITY; i <= 1.0; i += 1.0 / QUALITY) {
                            float2 blurPosition = finalUv + float2(cos(angle), sin(angle)) * blurRad * i;
                            color += sample(iImage1, blurPosition);
                        }
                    }
                    color /= QUALITY * NUM_DIRS;

                    return color;
                }

                ";


            var effect = SkSl.Compile(shaderCode);

            SKBitmap bitmap = null; //todo load
            SKShader textureShader = SKShader.CreateBitmap(bitmap, SKShaderTileMode.Clamp, SKShaderTileMode.Clamp);

            float timeValue = 1.0f;
            SKSize iResolution = new(100, 100);

            var uniforms = new SKRuntimeEffectUniforms(effect);
            uniforms.Add("iTime", 1.5f);
            uniforms.Add("iResolution", new float[] { iResolution.Width, iResolution.Height });

            SKRuntimeEffectChildren children = new SKRuntimeEffectChildren(effect);
            children.Add("iImage1", textureShader);

            SKShader shader = effect.ToShader(true, uniforms, children);
        }

        public MainPageShader()
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

        void Test()
        {
            string shaderCode = SkSl.LoadFromResources("Shaders/blur.sksl");
            var effect = SkSl.Compile(shaderCode);
        }


        //private void OnMauiTapped(object sender, EventArgs e)
        //{

        //    MainThread.BeginInvokeOnMainThread(() =>
        //    {
        //        var existing = MauiStack.Children.FirstOrDefault(x => x is SomeContent);
        //        if (existing != null)
        //        {
        //            MauiStack.Children.Remove(existing);
        //        }
        //        else
        //        {
        //            MauiStack.Children.Add(new SomeContent());
        //        }
        //    });

        //}

    }
}