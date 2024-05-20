single assembly to be able to use only one xaml markup ""draw:"

namespace DrawnUi.Maui.Draw 

- is the engine core
- contains base skia controls

namespace DrawnUi.Maui.Controls 

- contains optional subclassed controls

ToDo:

Tactical High-priority:

rewrite SkiaLabel measuring to use memory spans
add base scroll bar control as optional to attached to skiaScroll whatever

Tactical Low-priority:

add ChatList demo case
add default content for appropriate controls (SkiaButton etc) with optional type like enum values Platform (different upon platform), Material etc..
add SkiaLayout Flex-like, Masonry, think about porting StackPanel

Strategic Mid-priority:

port to SkiaSharp 3.0 (breaking changes)
add Maui.Graphics support
after that implement DrawnUi.Blazor

Strategic Low-proirity:

add tests and benchmarks, at pre-alpha majority of tests would break like all the time due to non-stop breaking changes