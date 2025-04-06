using System.ComponentModel;

namespace DrawnUi.Draw
{
    /// <summary>
    /// Provides extension methods for fluent API design pattern with DrawnUI controls
    /// </summary>
    public static partial class FluentExtensions
    {
        #region LOGIC

        /// <summary>
        /// Assigns the control to a variable and returns the control to continue the fluent chain
        /// </summary>
        /// <typeparam name="T">Type of SkiaControl</typeparam>
        /// <param name="control">The control to assign</param>
        /// <param name="variable">The out variable to store the reference</param>
        /// <returns>The control for chaining</returns>
        public static T Assign<T>(this T control, out T variable) where T : SkiaControl
        {
            variable = control;
            return control;
        }

        public static T AssignTo<T>(this T control, out T variable) where T : VisualElement
        {
            variable = control;
            return control;
        }

        /// <summary>
        /// Performs an action on the control and returns it to continue the fluent chain
        /// </summary>
        /// <typeparam name="T">Type of SkiaControl</typeparam>
        /// <param name="view">The control to act upon</param>
        /// <param name="action">The action to perform</param>
        /// <returns>The control for chaining</returns>
        public static T Adapt<T>(this T view, Action<T> action) where T : SkiaControl
        {
            try
            {
                action?.Invoke(view);
            }
            catch (Exception e)
            {
                Super.Log(e);
            }

            return view;
        }

        public static T SubscribeDebug<T, TSource>(
            this T control,
            TSource target,
            Action<T, string> callback,
            string[] propertyFilter = null)
            where T : SkiaControl
            where TSource : INotifyPropertyChanged
        {
            return control.Subscribe(target, callback, propertyFilter);
        }

        /// <summary>
        /// Subscribes to property changes on a source control and executes a callback when they occur.
        /// </summary>
        /// <typeparam name="T">Type of the target control (the one being extended)</typeparam>
        /// <typeparam name="TSource">Type of the source control (the one being observed)</typeparam>
        /// <param name="control">The control subscribing to changes</param>
        /// <param name="target">The control being observed</param>
        /// <param name="callback">Callback that receives the property name when changed</param>
        /// <param name="propertyFilter">Optional filter to only trigger on specific properties</param>
        /// <returns>The target control for chaining</returns>
        public static T Subscribe<T, TSource>(
            this T control,
            TSource target,
            Action<T, string> callback,
            string[] propertyFilter = null)
            where T : SkiaControl
            where TSource : INotifyPropertyChanged
        {
            // Create a unique key for this subscription
            string subscriptionKey = $"Subscribe_{target.GetHashCode()}_{Guid.NewGuid()}";

            // Create the handler
            PropertyChangedEventHandler handler = (sender, args) =>
            {
                // If a filter is specified, only proceed if the property is in the filter
                if (propertyFilter != null && !propertyFilter.Contains(args.PropertyName))
                    return;

                try
                {
                    callback(control, args.PropertyName);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Subscribe: Error in ViewModel property changed callback: {ex.Message}");
                }

            };

            // Subscribe to the event
            target.PropertyChanged += handler;

            // Will unsubscrbe when control is disposed 
            control.ExecuteUponDisposal[subscriptionKey] = () => { target.PropertyChanged -= handler; };

            return control;
        }

      

        /// <summary>
         /// Watches for property changes on the control's BindingContext of type TSource.
         /// Works with both immediate and delayed BindingContext assignment scenarios.
         /// </summary>
         /// <typeparam name="T">Type of the control</typeparam>
         /// <typeparam name="TSource">Expected type of the BindingContext</typeparam>
         /// <param name="control">The control to watch</param>
         /// <param name="callback">Callback executed when properties change, receiving the control, the typed BindingContext, and the property name</param>
         /// <returns>The control for chaining</returns>
         /// <remarks>
         /// This method handles two scenarios:
         /// 1. The BindingContext is already set when the method is called
         /// 2. The BindingContext will be set sometime after the method is called
         /// 
         /// The callback will be invoked immediately after subscription with an empty property name,
         /// allowing initialization based on the current state.
         /// </remarks>
        public static T WatchBindingContext<T, TSource>(
            this T control,
            Action<T, TSource, string> callback)
            where T : SkiaControl
            where TSource : INotifyPropertyChanged
        {
            // Local method to handle subscription and initial call
            void SubscribeToViewModel(TSource tvm)
            {
                // Subscribe directly
                Subscribe(control, tvm, (me, prop) =>
                {
                    InvokeCallback(me, tvm, prop);
                });

                // Initial call with empty property name
                InvokeCallback(control, tvm, nameof(SkiaControl.BindingContext));
            }

            // Local method to safely invoke the callback
            void InvokeCallback(T ctrl, TSource vm, string prop)
            {
                try
                {
                    callback.Invoke(ctrl, vm, prop);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"WatchBindingContext: Error in ViewModel property changed callback: {ex.Message}");
                }
            }

            // Check if BindingContext is already set
            if (control.BindingContext is TSource tvm)
            {
                SubscribeToViewModel(tvm);
            }
            else
            {
                // Set up subscription for when BindingContext changes
                string subscriptionKey = $"watch_{Guid.NewGuid()}";

                void ControlOnApplyingBindingContext(object sender, EventArgs e)
                {
                    if (control.BindingContext is TSource tvm)
                    {
                        // Clean up the temporary event handler
                        control.ApplyingBindingContext -= ControlOnApplyingBindingContext;
                        control.ExecuteUponDisposal.Remove(subscriptionKey);

                        // Set up the actual subscription
                        SubscribeToViewModel(tvm);
                    }
                }

                // Register the temporary event handler and its cleanup
                control.ApplyingBindingContext += ControlOnApplyingBindingContext;
                control.ExecuteUponDisposal[subscriptionKey] = () => {
                    control.ApplyingBindingContext -= ControlOnApplyingBindingContext;
                };
            }

            return control;
        }

        /// <summary>
        /// Watches for property changes on another control's BindingContext of type TSource.
        /// </summary>
        /// <typeparam name="T">Type of the control being extended</typeparam>
        /// <typeparam name="TTarget">Type of the target control whose BindingContext we're watching</typeparam>
        /// <typeparam name="TSource">Expected type of the target control's BindingContext</typeparam>
        /// <param name="control">The control to extend</param>
        /// <param name="target">The target control whose BindingContext to watch</param>
        /// <param name="callback">Callback executed when properties change, receiving the control, the target control, the typed BindingContext, and the property name</param>
        /// <returns>The original control for chaining</returns>
        /// <remarks>
        /// This method handles two scenarios:
        /// 1. The target's BindingContext is already set when the method is called
        /// 2. The target's BindingContext will be set sometime after the method is called
        /// </remarks>
        public static T WatchTargetBindingContext<T, TTarget, TSource>(
            this T control,
            TTarget target,
            Action<T, TTarget, TSource, string> callback)
            where T : SkiaControl
            where TTarget : SkiaControl
            where TSource : INotifyPropertyChanged
        {
            // Local method to handle subscription and initial call
            void SubscribeToViewModel(TSource tvm)
            {
                // Subscribe directly
                Subscribe(control, tvm, (me, prop) =>
                {
                    InvokeCallback(me, target, tvm, prop);
                });

                // Initial call with empty property name
                InvokeCallback(control, target, tvm, nameof(SkiaControl.BindingContext));
            }

            // Local method to safely invoke the callback
            void InvokeCallback(T ctrl, TTarget tgt, TSource vm, string prop)
            {
                try
                {
                    callback.Invoke(ctrl, tgt, vm, prop);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"WatchOtherBindingContext: Error in ViewModel property changed callback: {ex.Message}");
                }
            }

            // Check if the target's BindingContext is already set
            if (target.BindingContext is TSource tvm)
            {
                SubscribeToViewModel(tvm);
            }
            else
            {
                // Set up subscription for when the target's BindingContext changes
                string subscriptionKey = $"watch_other_{Guid.NewGuid()}";

                void TargetOnApplyingBindingContext(object sender, EventArgs e)
                {
                    if (target.BindingContext is TSource tvm)
                    {
                        // Clean up the temporary event handler
                        target.ApplyingBindingContext -= TargetOnApplyingBindingContext;
                        control.ExecuteUponDisposal.Remove(subscriptionKey);

                        // Set up the actual subscription
                        SubscribeToViewModel(tvm);
                    }
                }

                // Register the temporary event handler and its cleanup
                target.ApplyingBindingContext += TargetOnApplyingBindingContext;
                control.ExecuteUponDisposal[subscriptionKey] = () => {
                    target.ApplyingBindingContext -= TargetOnApplyingBindingContext;
                };
            }

            return control;
        }

        /// <summary>
        /// Simple non-compiled binding
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="view"></param>
        /// <param name="property"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T BindProperty<T>(this T view, BindableProperty property, string path) where T : SkiaControl
        {
            view.SetBinding(property, path);

            return view;
        }


        #endregion

        #region UI

        /// <summary>
        /// Adds multiple child controls to a layout
        /// </summary>
        /// <typeparam name="T">Type of SkiaLayout</typeparam>
        /// <param name="view">The layout to add children to</param>
        /// <param name="children">The children to add</param>
        /// <returns>The layout for chaining</returns>
        public static T WithChildren<T>(this T view, params SkiaControl[] children) where T : SkiaLayout
        {
            foreach (SkiaControl child in children)
            {
                view.AddSubView(child);
            }

            return view;
        }

        /// <summary>
        /// Sets the content of a container that implements IWithContent
        /// </summary>
        /// <typeparam name="T">Type implementing IWithContent</typeparam>
        /// <param name="view">The container</param>
        /// <param name="child">The content to set</param>
        /// <returns>The container for chaining</returns>
        public static T WithContent<T>(this T view, SkiaControl child) where T : IWithContent
        {
            view.Content = child;
            return view;
        }

        /// <summary>
        /// Adds the control to a parent and returns the control for further chaining
        /// </summary>
        /// <typeparam name="T">Type of SkiaControl</typeparam>
        /// <param name="view">The control to add</param>
        /// <param name="parent">The parent to add the control to</param>
        /// <returns>The control for chaining</returns>
        public static T WithParent<T>(this T view, IDrawnBase parent) where T : SkiaControl
        {
            parent.AddSubView(view);
            return view;
        }

        /// <summary>
        /// Sets the control's horizontal options to center
        /// </summary>
        /// <typeparam name="T">Type of SkiaControl</typeparam>
        /// <param name="view">The control to center horizontally</param>
        /// <returns>The control for chaining</returns>
        public static T CenterX<T>(this T view) where T : SkiaControl
        {
            view.HorizontalOptions = LayoutOptions.Center;
            return view;
        }

        /// <summary>
        /// Sets the control's vertical options to center
        /// </summary>
        /// <typeparam name="T">Type of SkiaControl</typeparam>
        /// <param name="view">The control to center vertically</param>
        /// <returns>The control for chaining</returns>
        public static T CenterY<T>(this T view) where T : SkiaControl
        {
            view.VerticalOptions = LayoutOptions.Center;
            return view;
        }

        /// <summary>
        /// Fills in both directions
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="view"></param>
        /// <returns></returns>
        public static T Fill<T>(this T view) where T : SkiaControl
        {
            view.HorizontalOptions = LayoutOptions.Fill;
            view.VerticalOptions = LayoutOptions.Fill;
            return view;
        }

        /// <summary>
        /// Centers the control both horizontally and vertically
        /// </summary>
        /// <typeparam name="T">Type of SkiaControl</typeparam>
        /// <param name="view">The control to center</param>
        /// <returns>The control for chaining</returns>
        public static T Center<T>(this T view) where T : SkiaControl
        {
            view.HorizontalOptions = LayoutOptions.Center;
            view.VerticalOptions = LayoutOptions.Center;
            return view;
        }

        /// <summary>
        /// Sets the margin for the control
        /// </summary>
        /// <typeparam name="T">Type of SkiaControl</typeparam>
        /// <param name="view">The control to set margin for</param>
        /// <param name="uniformMargin">The uniform margin to apply to all sides</param>
        /// <returns>The control for chaining</returns>
        public static T WithMargin<T>(this T view, double uniformMargin) where T : SkiaControl
        {
            view.Margin = new Thickness(uniformMargin);
            return view;
        }

        /// <summary>
        /// Sets the margin for the control
        /// </summary>
        /// <typeparam name="T">Type of SkiaControl</typeparam>
        /// <param name="view">The control to set margin for</param>
        /// <param name="horizontal">The left and right margin</param>
        /// <param name="vertical">The top and bottom margin</param>
        /// <returns>The control for chaining</returns>
        public static T WithMargin<T>(this T view, double horizontal, double vertical) where T : SkiaControl
        {
            view.Margin = new Thickness(horizontal, vertical);
            return view;
        }

        /// <summary>
        /// Sets the margin for the control
        /// </summary>
        /// <typeparam name="T">Type of SkiaControl</typeparam>
        /// <param name="view">The control to set margin for</param>
        /// <param name="left">The left margin</param>
        /// <param name="top">The top margin</param>
        /// <param name="right">The right margin</param>
        /// <param name="bottom">The bottom margin</param>
        /// <returns>The control for chaining</returns>
        public static T WithMargin<T>(this T view, double left, double top, double right, double bottom)
            where T : SkiaControl
        {
            view.Margin = new Thickness(left, top, right, bottom);
            return view;
        }

        /// <summary>
        /// Sets the padding for a layout control
        /// </summary>
        /// <typeparam name="T">Type of SkiaLayout</typeparam>
        /// <param name="view">The layout to set padding for</param>
        /// <param name="uniformPadding">The uniform padding to apply to all sides</param>
        /// <returns>The layout for chaining</returns>
        public static T WithPadding<T>(this T view, double uniformPadding) where T : SkiaLayout
        {
            view.Padding = new Thickness(uniformPadding);
            return view;
        }

        /// <summary>
        /// Sets the padding for a layout control
        /// </summary>
        /// <typeparam name="T">Type of SkiaLayout</typeparam>
        /// <param name="view">The layout to set padding for</param>
        /// <param name="horizontal">The left and right padding</param>
        /// <param name="vertical">The top and bottom padding</param>
        /// <returns>The layout for chaining</returns>
        public static T WithPadding<T>(this T view, double horizontal, double vertical) where T : SkiaLayout
        {
            view.Padding = new Thickness(horizontal, vertical);
            return view;
        }

        /// <summary>
        /// Sets the padding for a layout control
        /// </summary>
        /// <typeparam name="T">Type of SkiaLayout</typeparam>
        /// <param name="view">The layout to set padding for</param>
        /// <param name="left">The left padding</param>
        /// <param name="top">The top padding</param>
        /// <param name="right">The right padding</param>
        /// <param name="bottom">The bottom padding</param>
        /// <returns>The layout for chaining</returns>
        public static T WithPadding<T>(this T view, double left, double top, double right, double bottom)
            where T : SkiaLayout
        {
            view.Padding = new Thickness(left, top, right, bottom);
            return view;
        }

        /// <summary>
        /// Sets the Tag property for the control
        /// </summary>
        /// <typeparam name="T">Type of SkiaControl</typeparam>
        /// <param name="view">The control to set the Tag for</param>
        /// <param name="tag">The tag value</param>
        /// <returns>The control for chaining</returns>
        public static T WithTag<T>(this T view, string tag) where T : SkiaControl
        {
            view.Tag = tag;
            return view;
        }

        #endregion
    }
}
