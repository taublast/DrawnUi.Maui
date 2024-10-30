#if DEBUG
[assembly: System.Reflection.Metadata.MetadataUpdateHandlerAttribute(typeof(DrawnUi.HotReloadService))]
namespace DrawnUi
{
    public static class HotReloadService
    {
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        public static event Action<Type[]?>? UpdateApplicationEvent;
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.

        internal static void ClearCache(Type[]? types) { }

        internal static void UpdateApplication(Type[]? types)
        {
            UpdateApplicationEvent?.Invoke(types); //i'm saving my page and this triggers around 5 times intead on 1, Type inside is same, of the page in question.

            Super.NeedGlobalUpdate();
        }

    }
}
#endif

/*
class HotReloadHandler
{
    public async void OnHotReload(IReadOnlyList<Type> types)
    {
        if (Application.Current?.Windows is null)
        {
            Debug.WriteLine($"{nameof(HotReloadHandler)} Failed: {nameof(Application)}.{nameof(Application.Current)}.{nameof(Application.Current.Windows)} is null");
            return;
        }

        foreach (var window in Application.Current.Windows)
        {
            if (window.Page is not Page currentPage)
            {
                return;
            }

            foreach (var type in types)
            {
                if (type.IsSubclassOf(typeof(Page)))
                {
                    if (window.Page is AppShell shell)
                    {
                        if (shell.CurrentPage is Page visiblePage
                            && visiblePage.GetType() == type)
                        {
                            var currentPageShellRoute = AppShell.GetRoute(type);

                            await currentPage.Dispatcher.DispatchAsync(async () =>
                            {
                                await shell.GoToAsync(currentPageShellRoute, false);
                                shell.Navigation.RemovePage(visiblePage);
                            });

                            break;
                        }
                    }
                    else
                    {
                        if (TryGetModalStackPage(window, out var modalPage))
                        {
                            await currentPage.Dispatcher.DispatchAsync(async () =>
                            {
                                await currentPage.Navigation.PopModalAsync(false);
                                await currentPage.Navigation.PushModalAsync(modalPage, false);
                            });
                        }
                        else
                        {
                            await currentPage.Dispatcher.DispatchAsync(async () =>
                            {
                                await currentPage.Navigation.PopAsync(false);
                                await currentPage.Navigation.PushAsync(modalPage, false);
                            });
                        }

                        break;
                    }
                }
            }
        }
    }


    static bool TryGetModalStackPage(Window window, [NotNullWhen(true)] out Page? page)
    {
        page = window.Navigation.ModalStack.LastOrDefault();
        return page is not null;
    }
}

*/