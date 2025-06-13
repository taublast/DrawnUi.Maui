using DrawnUi.Camera;
using DrawnUi.Infrastructure;

namespace AppoMobi.Maui.DrawnUi.Demo.Views
{
    public class CameraWithEffects : SkiaCamera
    {

        public CameraWithEffects()
        {
            InitializeAvailableShaders();
        }

        protected override void OnDisplayReady()
        {
            base.OnDisplayReady();

            //Display.UseCache = SkiaCacheType.GPU; 

            InitializeEffects();
        }

        protected override void Paint(DrawingContext ctx)
        {
            base.Paint(ctx);

            FrameAquired = false;
        }

        /// <summary>
        /// Initialize camera effects - sets starting effect
        /// </summary>
        public void InitializeEffects()
        {
            InitializeAvailableShaders();

            SetEffect(SkiaImageEffect.Custom); // <=== DEFAULT AT STARTUP
        }

        public readonly List<SkiaImageEffect> AvailableEffects = new List<SkiaImageEffect>()
        {
            SkiaImageEffect.Custom,
            SkiaImageEffect.Sepia,
            SkiaImageEffect.BlackAndWhite,
            SkiaImageEffect.Pastel,
            SkiaImageEffect.None,
        };

        private static string path = @"Shaders\Camera";
        private static List<string> _shaders;

        static void InitializeAvailableShaders()
        {
            if (_shaders == null)
            {
                _shaders = Files.ListAssets(path);

            }
        }


        private SkiaShaderEffect _shader;

        //protected override SkiaImage CreatePreview()
        //{
        //    var display = base.CreatePreview();


        //}

        //public class CameraDisplayWrapper : SkiaImage
        //{
        //    public override SKImage CachedImage
        //    {
        //        get
        //        {
        //            return LoadedSource?.Image;
        //        }
        //    }
        //}

        public void SetEffect(SkiaImageEffect effect)
        {
            if (Display == null)
            {
                return;
            }

            if (effect == SkiaImageEffect.Custom)
            {
                if (_shader == null)
                {
                    _shader = new SkiaShaderEffect()
                    {
                        ShaderSource = "Shaders/Camera/old.sksl",
                        //FilterMode = SKFilterMode.Linear <== it's default
                    };
                }

                if (_shader != null && !VisualEffects.Contains(_shader))
                {
                    VisualEffects.Add(_shader);
                }
            }
            else
            {
                if (_shader != null && VisualEffects.Contains(_shader))
                {
                    VisualEffects.Remove(_shader);
                }
            }
            Effect = effect;
        }

        public void SetCustomShader(string shaderFilename)
        {
            if (Display == null)
            {
                return;
            }

            // Remove existing shader if any
            if (_shader != null && VisualEffects.Contains(_shader))
            {
                VisualEffects.Remove(_shader);
            }

            // Create new shader with the specified filename
            _shader = new SkiaShaderEffect()
            {
                ShaderSource = shaderFilename,
                //FilterMode = SKFilterMode.Linear <== it's default
            };

            // Add the new shader
            if (_shader != null && !VisualEffects.Contains(_shader))
            {
                VisualEffects.Add(_shader);
            }

            // Set effect to custom to enable shader
            Effect = SkiaImageEffect.Custom;
        }

    }
}
