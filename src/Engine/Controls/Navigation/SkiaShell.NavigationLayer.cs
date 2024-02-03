using System.Collections.ObjectModel;

namespace DrawnUi.Maui.Controls;

public partial class SkiaShell
{
    public class NavigationLayer<T> where T : SkiaControl
    {

        /// <summary>
        /// if isModel is true than will try to freeze background before showing. otherwise will be just an overlay like toast etc.
        /// </summary>
        /// <param name="shell"></param>
        /// <param name="isModal"></param>
        public NavigationLayer(SkiaShell shell, bool freezeLayout)
        {
            _shell = shell;
            _freezeLayout = freezeLayout;
        }

        public ObservableCollection<T> NavigationStack = new ObservableCollection<T>();

        protected readonly SkiaShell _shell;

        private readonly bool _freezeLayout;

        public virtual async Task Open(T control, bool animated)
        {
            if (_freezeLayout)
            {
                await _shell.FreezeRootLayout(control, animated);
            }
            try
            {
                if (control is IVisibilityAware aware)
                {
                    aware.OnAppearing();
                }

                _shell.ShellLayout.AddSubView(control);
                //control.SetParent(_shell.ShellLayout);

                NavigationStack.Add(control);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }

        }

        public virtual void OnOpened(T control)
        {
            //popup.SetRendered(true);
            if (control is IVisibilityAware aware)
            {
                aware.OnAppearing();
            }
        }

        public async Task Close(T control, bool animated)
        {
            try
            {
                if (control is IVisibilityAware aware)
                {
                    aware.OnDisappearing();
                }

                NavigationStack.Remove(control);

                //todo dispose?..
                //App.Current.Layout.Update();

                if (_freezeLayout)
                {
                    await _shell.UnfreezeRootLayout(control, animated);
                }

                control.SetParent(null);

                control.Dispose();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                //App.Current.ToastShortMessage(ResStrings.Error);
            }
        }

        public async Task Close(bool animated)
        {
            var topmost = NavigationStack.LastOrDefault();
            if (topmost != null)
            {
                await Close(topmost, animated);
            }
        }

        public async Task CloseAll()
        {
            try
            {
                foreach (var control in NavigationStack)
                {
                    control.IsVisible = false;
                    if (control is IVisibilityAware aware)
                    {
                        aware.OnDisappearing();
                    }
                }
                NavigationStack.Clear();

                //App.Current.Layout.Update();
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                //App.Current.ToastShortMessage(ResStrings.Error);
            }

        }


    }
}