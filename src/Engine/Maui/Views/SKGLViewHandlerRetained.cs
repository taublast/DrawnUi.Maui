using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Handlers;

namespace DrawnUi.Views
{
#if WINDOWS || MACCATALYST || IOS || ANDROID

    /// <summary>
    /// SHARED
    /// </summary>
    public partial class SKGLViewHandlerRetained
    {
        public static PropertyMapper<ISKGLView, SKGLViewHandlerRetained> SKGLViewMapper =
            new PropertyMapper<ISKGLView, SKGLViewHandlerRetained>(ViewHandler.ViewMapper)
            {
                [nameof(ISKGLView.EnableTouchEvents)] = MapEnableTouchEvents,
                [nameof(ISKGLView.IgnorePixelScaling)] = MapIgnorePixelScaling,
                [nameof(ISKGLView.HasRenderLoop)] = MapHasRenderLoop,
#if WINDOWS
				[nameof(ISKGLView.Background)] = MapBackground,
#endif
            };

        public static CommandMapper<ISKGLView, SKGLViewHandlerRetained> SKGLViewCommandMapper =
            new CommandMapper<ISKGLView, SKGLViewHandlerRetained>(ViewHandler.ViewCommandMapper)
            {
                [nameof(ISKGLView.InvalidateSurface)] = OnInvalidateSurface,
            };

        public SKGLViewHandlerRetained()
            : base(SKGLViewMapper, SKGLViewCommandMapper)
        {
        }

        public SKGLViewHandlerRetained(PropertyMapper? mapper, CommandMapper? commands)
            : base(mapper ?? SKGLViewMapper, commands ?? SKGLViewCommandMapper)
        {
        }
    }

#endif
}
