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

        /// <summary>
        /// This will be executed ones along and just before the CreateDefaultContent. This lets you execute initialization code after the control is already in the view tree and all variables you might want to use are already filled.
        /// </summary>
        /// <typeparam name="T">Represents a type that extends SkiaControl, allowing for specific control initialization.</typeparam>
        /// <param name="view">The control instance that will be initialized with the provided action.</param>
        /// <param name="action">An operation to perform on the control instance during initialization.</param>
        /// <returns>The initialized control instance after the action has been applied.</returns>
        public static T Initialize<T>(this T view, Action<T> action) where T : SkiaControl
        {
            view.ExecuteAfterCreated[Guid.NewGuid().ToString()] = control =>
            {
                action.Invoke((T)control);
            };
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
            return control.Observe(target, callback, propertyFilter);
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
        public static T Observe<T, TSource>(
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
        /// <param name="debugTypeMismatch">Whether to log a warning when the actual BindingContext type doesn't match TSource (default: true)</param>
        /// <returns>The control for chaining</returns>
        /// <remarks>
        /// This method handles two scenarios:
        /// 1. The BindingContext is already set when the method is called
        /// 2. The BindingContext will be set sometime after the method is called
        /// 
        /// The callback will be invoked immediately after subscription with an empty property name,
        /// allowing initialization based on the current state.
        /// </remarks>
        public static T ObserveBindingContext<T, TSource>(
            this T control,
            Action<T, TSource, string> callback,
            bool debugTypeMismatch = true)
            where T : SkiaControl
            where TSource : INotifyPropertyChanged
        {
            // Local method to handle subscription and initial call
            void SubscribeToViewModel(TSource tvm)
            {
                // Subscribe directly
                Observe(control, tvm, (me, prop) =>
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
            else if (control.BindingContext != null && debugTypeMismatch)
            {
                // BindingContext exists but is of the wrong type - log a warning
                Trace.WriteLine($"[WARNING] ObserveBindingContext: Expected BindingContext type {typeof(TSource).Name} but got {control.BindingContext.GetType().Name} for control {control.GetType().Name}");
            }

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
                else if (control.BindingContext != null && debugTypeMismatch)
                {
                    // BindingContext changed but is still the wrong type - log a warning
                    Trace.WriteLine($"[WARNING] ObserveBindingContext: Expected BindingContext type {typeof(TSource).Name} but got {control.BindingContext.GetType().Name} for control {control.GetType().Name}");
                }
            }

            // Register the temporary event handler and its cleanup
            control.ApplyingBindingContext += ControlOnApplyingBindingContext;
            control.ExecuteUponDisposal[subscriptionKey] = () => {
                control.ApplyingBindingContext -= ControlOnApplyingBindingContext;
            };

            return control;
        }

        /// <summary>
        /// Observes a deeply nested property on the control's BindingContext in a type-safe manner.
        /// </summary>
        /// <typeparam name="T">Type of the control</typeparam>
        /// <typeparam name="TSource">Expected type of the BindingContext</typeparam>
        /// <typeparam name="TIntermediate1">Type of the first intermediate object</typeparam>
        /// <typeparam name="TIntermediate2">Type of the second intermediate object</typeparam>
        /// <typeparam name="TProperty">Type of the final property</typeparam>
        /// <param name="control">The control to watch</param>
        /// <param name="intermediate1Selector">Function to select the first intermediate object</param>
        /// <param name="intermediate1PropertyName">Name of the first intermediate property</param>
        /// <param name="intermediate2Selector">Function to select the second intermediate object</param>
        /// <param name="intermediate2PropertyName">Name of the second intermediate property</param>
        /// <param name="propertySelector">Function to select the final property</param>
        /// <param name="propertyName">Name of the final property</param>
        /// <param name="callback">Callback that receives the control and current property value</param>
        /// <param name="defaultValue">Default value to use when any intermediate is null</param>
        /// <param name="debugTypeMismatch">Whether to log warnings for type mismatches</param>
        /// <returns>The control for chaining</returns>
        public static T ObserveDeepNestedProperty<T, TSource, TIntermediate1, TIntermediate2, TProperty>(
            this T control,
            Func<TSource, TIntermediate1> intermediate1Selector,
            string intermediate1PropertyName,
            Func<TIntermediate1, TIntermediate2> intermediate2Selector,
            string intermediate2PropertyName,
            Func<TIntermediate2, TProperty> propertySelector,
            string propertyName,
            Action<T, TProperty> callback,
            TProperty defaultValue = default,
            bool debugTypeMismatch = true)
            where T : SkiaControl
            where TSource : INotifyPropertyChanged
            where TIntermediate1 : class, INotifyPropertyChanged
            where TIntermediate2 : class, INotifyPropertyChanged
        {
            // Dictionary to track all subscriptions for cleanup
            Dictionary<string, PropertyChangedEventHandler> subscriptions = new Dictionary<string, PropertyChangedEventHandler>();
            string mainKey = $"ObserveDeepNested_{Guid.NewGuid()}";

            // Helper method to safely invoke callback
            void InvokeCallback(T ctrl, TProperty value)
            {
                try
                {
                    callback(ctrl, value);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"ObserveDeepNestedProperty: Error in callback: {ex.Message}");
                }
            }

            // Helper to clean up a specific subscription
            void CleanupSubscription(string key, object target)
            {
                if (subscriptions.TryGetValue(key, out var handler) && target != null)
                {
                    if (target is INotifyPropertyChanged notifyTarget)
                    {
                        notifyTarget.PropertyChanged -= handler;
                    }
                    subscriptions.Remove(key);
                }
            }

            // Helper to observe the final property
            void ObserveFinalProperty(T ctrl, TIntermediate2 intermediate2)
            {
                // If intermediate2 is null, call callback with default value and skip subscription
                if (intermediate2 == null)
                {
                    InvokeCallback(ctrl, defaultValue);
                    return;
                }

                // Clean up any existing subscription for this level
                CleanupSubscription($"Final_{intermediate2.GetHashCode()}", intermediate2);

                // Create new subscription
                PropertyChangedEventHandler handler = (sender, args) =>
                {
                    if (string.IsNullOrEmpty(args.PropertyName) || args.PropertyName == propertyName)
                    {
                        try
                        {
                            TProperty value = propertySelector(intermediate2);
                            InvokeCallback(ctrl, value);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine($"ObserveDeepNestedProperty: Error accessing final property: {ex.Message}");
                        }
                    }
                };

                // Register the handler
                intermediate2.PropertyChanged += handler;
                subscriptions[$"Final_{intermediate2.GetHashCode()}"] = handler;

                // Initial callback
                try
                {
                    TProperty value = propertySelector(intermediate2);
                    InvokeCallback(ctrl, value);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"ObserveDeepNestedProperty: Error in initial final callback: {ex.Message}");
                }
            }

            // Helper to observe the second level
            void ObserveSecondLevel(T ctrl, TIntermediate1 intermediate1)
            {
                // If intermediate1 is null, call callback with default value and skip subscription
                if (intermediate1 == null)
                {
                    InvokeCallback(ctrl, defaultValue);
                    return;
                }

                // Clean up any existing subscription for this level
                CleanupSubscription($"Second_{intermediate1.GetHashCode()}", intermediate1);

                // Create new subscription
                PropertyChangedEventHandler handler = (sender, args) =>
                {
                    if (string.IsNullOrEmpty(args.PropertyName) || args.PropertyName == intermediate2PropertyName)
                    {
                        // Clean up any existing final property subscriptions
                        foreach (var key in subscriptions.Keys.Where(k => k.StartsWith("Final_")).ToList())
                        {
                            CleanupSubscription(key, null); // We don't have direct reference to the object
                        }

                        // Get new intermediate2 and observe it
                        TIntermediate2 intermediate2 = intermediate2Selector(intermediate1);
                        ObserveFinalProperty(ctrl, intermediate2);
                    }
                };

                // Register the handler
                intermediate1.PropertyChanged += handler;
                subscriptions[$"Second_{intermediate1.GetHashCode()}"] = handler;

                // Initial setup for second level
                TIntermediate2 intermediate2 = intermediate2Selector(intermediate1);
                ObserveFinalProperty(ctrl, intermediate2);
            }

            // Set up root observation
            control.ObserveBindingContext<T, TSource>((ctrl, vm, prop) =>
            {
                if (prop == nameof(SkiaControl.BindingContext) || string.IsNullOrEmpty(prop))
                {
                    // When BindingContext is assigned, set up the first observation level
                    TIntermediate1 intermediate1 = intermediate1Selector(vm);
                    ObserveSecondLevel(ctrl, intermediate1);

                    // Also set up subscription to intermediate1 property changes
                    CleanupSubscription("Root", vm);

                    PropertyChangedEventHandler rootHandler = (sender, args) =>
                    {
                        if (string.IsNullOrEmpty(args.PropertyName) || args.PropertyName == intermediate1PropertyName)
                        {
                            // Clean up any existing second level subscriptions
                            foreach (var key in subscriptions.Keys.Where(k => k.StartsWith("Second_") || k.StartsWith("Final_")).ToList())
                            {
                                CleanupSubscription(key, null); // We don't have direct reference to the object
                            }

                            // Get new intermediate1 and observe it
                            TIntermediate1 newIntermediate1 = intermediate1Selector(vm);
                            ObserveSecondLevel(ctrl, newIntermediate1);
                        }
                    };

                    vm.PropertyChanged += rootHandler;
                    subscriptions["Root"] = rootHandler;
                }
                else if (prop == intermediate1PropertyName)
                {
                    // The intermediate1 property changed on the ViewModel
                    TIntermediate1 newIntermediate1 = intermediate1Selector(vm);

                    // Clean up any existing second level subscriptions
                    foreach (var key in subscriptions.Keys.Where(k => k.StartsWith("Second_") || k.StartsWith("Final_")).ToList())
                    {
                        CleanupSubscription(key, null);
                    }

                    ObserveSecondLevel(ctrl, newIntermediate1);
                }
            }, debugTypeMismatch);

            // Set up cleanup
            control.ExecuteUponDisposal[mainKey] = () =>
            {
                foreach (var subscription in subscriptions)
                {
                    if (subscription.Key == "Root" && control.BindingContext is TSource sourceT)
                    {
                        sourceT.PropertyChanged -= subscription.Value;
                    }
                    else if (subscription.Key.StartsWith("Second_") && control.BindingContext is TSource sourceT2)
                    {
                        var intermediate1 = intermediate1Selector(sourceT2);
                        if (intermediate1 != null)
                        {
                            intermediate1.PropertyChanged -= subscription.Value;
                        }
                    }
                    else if (subscription.Key.StartsWith("Final_") && control.BindingContext is TSource sourceVm)
                    {
                        var intermediate1 = intermediate1Selector(sourceVm);
                        if (intermediate1 != null)
                        {
                            var intermediate2 = intermediate2Selector(intermediate1);
                            if (intermediate2 != null)
                            {
                                intermediate2.PropertyChanged -= subscription.Value;
                            }
                        }
                    }
                }
                subscriptions.Clear();
            };

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
        /// <param name="debugTypeMismatch">Whether to log a warning when the actual BindingContext type doesn't match TSource (default: true)</param>
        /// <returns>The original control for chaining</returns>
        /// <remarks>
        /// This method handles two scenarios:
        /// 1. The target's BindingContext is already set when the method is called
        /// 2. The target's BindingContext will be set sometime after the method is called
        /// </remarks>
        public static T ObserveTargetBindingContext<T, TTarget, TSource>(
            this T control,
            TTarget target,
            Action<T, TTarget, TSource, string> callback,
            bool debugTypeMismatch = true)
            where T : SkiaControl
            where TTarget : SkiaControl
            where TSource : INotifyPropertyChanged
        {
            // Local method to handle subscription and initial call
            void SubscribeToViewModel(TSource tvm)
            {
                // Subscribe directly
                Observe(control, tvm, (me, prop) =>
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
            else if (target.BindingContext != null && debugTypeMismatch)
            {
                // BindingContext exists but is of the wrong type - log a warning
                Trace.WriteLine($"[WARNING] ObserveTargetBindingContext: Expected BindingContext type {typeof(TSource).Name} but got {target.BindingContext.GetType().Name} for target control {target.GetType().Name}");
            }

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
                else if (target.BindingContext != null && debugTypeMismatch)
                {
                    // BindingContext changed but is still the wrong type - log a warning
                    Trace.WriteLine($"[WARNING] ObserveTargetBindingContext: Expected BindingContext type {typeof(TSource).Name} but got {target.BindingContext.GetType().Name} for target control {target.GetType().Name}");
                }
            }

            // Register the temporary event handler and its cleanup
            target.ApplyingBindingContext += TargetOnApplyingBindingContext;
            control.ExecuteUponDisposal[subscriptionKey] = () => {
                target.ApplyingBindingContext -= TargetOnApplyingBindingContext;
            };

            return control;
        }



        /// <summary>
        /// Observes a nested property on the control's BindingContext in a type-safe manner.
        /// </summary>
        /// <typeparam name="T">Type of the control</typeparam>
        /// <typeparam name="TSource">Expected type of the BindingContext</typeparam>
        /// <typeparam name="TIntermediate">Type of the intermediate object</typeparam>
        /// <typeparam name="TProperty">Type of the final property</typeparam>
        /// <param name="control">The control to watch</param>
        /// <param name="intermediateSelector">Function to select the intermediate object</param>
        /// <param name="intermediatePropertyName">Name of the intermediate property</param>
        /// <param name="propertySelector">Function to select the final property</param>
        /// <param name="propertyName">Name of the final property</param>
        /// <param name="callback">Callback that receives the control and current property value</param>
        /// <param name="defaultValue">Default value to use when intermediate is null</param>
        /// <param name="debugTypeMismatch">Whether to log warnings for type mismatches</param>
        /// <returns>The control for chaining</returns>
        public static T ObserveNestedProperty<T, TSource, TIntermediate, TProperty>(
            this T control,
            Func<TSource, TIntermediate> intermediateSelector,
            string intermediatePropertyName,
            Func<TIntermediate, TProperty> propertySelector,
            string propertyName,
            Action<T, TProperty> callback,
            TProperty defaultValue = default,
            bool debugTypeMismatch = true)
            where T : SkiaControl
            where TSource : INotifyPropertyChanged
            where TIntermediate : class, INotifyPropertyChanged
        {
            // Dictionary to track all subscriptions for cleanup
            Dictionary<string, PropertyChangedEventHandler> subscriptions = new Dictionary<string, PropertyChangedEventHandler>();
            string mainKey = $"ObserveNested_{intermediatePropertyName}_{propertyName}_{Guid.NewGuid()}";

            // Helper method to safely invoke callback
            void InvokeCallback(T ctrl, TProperty value)
            {
                try
                {
                    callback(ctrl, value);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"ObserveNestedProperty: Error in callback: {ex.Message}");
                }
            }

            // Helper method to clean up an intermediate subscription
            void CleanupIntermediateSubscription(TIntermediate intermediate)
            {
                string key = $"Intermediate_{intermediate?.GetHashCode()}";
                if (subscriptions.TryGetValue(key, out var handler) && intermediate != null)
                {
                    intermediate.PropertyChanged -= handler;
                    subscriptions.Remove(key);
                }
            }

            // Helper method to observe the nested property
            void ObserveIntermediateProperty(T ctrl, TIntermediate intermediate)
            {
                // If intermediate is null, call callback with default value and skip subscription
                if (intermediate == null)
                {
                    InvokeCallback(ctrl, defaultValue);
                    return;
                }

                // Clean up any existing subscription for this intermediate
                CleanupIntermediateSubscription(intermediate);

                // Create new subscription
                PropertyChangedEventHandler handler = (sender, args) =>
                {
                    if (string.IsNullOrEmpty(args.PropertyName) || args.PropertyName == propertyName)
                    {
                        try
                        {
                            TProperty value = propertySelector(intermediate);
                            InvokeCallback(ctrl, value);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine($"ObserveNestedProperty: Error accessing property: {ex.Message}");
                        }
                    }
                };

                // Register the handler
                intermediate.PropertyChanged += handler;
                subscriptions[$"Intermediate_{intermediate.GetHashCode()}"] = handler;

                // Initial callback
                try
                {
                    TProperty value = propertySelector(intermediate);
                    InvokeCallback(ctrl, value);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine($"ObserveNestedProperty: Error in initial callback: {ex.Message}");
                }
            }

            // Set up root observation
            control.ObserveBindingContext<T, TSource>((ctrl, vm, prop) =>
            {
                if (prop == nameof(SkiaControl.BindingContext) || string.IsNullOrEmpty(prop))
                {
                    // When BindingContext is assigned, set up the first observation level
                    var intermediate = intermediateSelector(vm);
                    ObserveIntermediateProperty(ctrl, intermediate);

                    // Also set up subscription to intermediate property changes
                    if (subscriptions.TryGetValue("Root", out var oldHandler))
                    {
                        vm.PropertyChanged -= oldHandler;
                    }

                    PropertyChangedEventHandler rootHandler = (sender, args) =>
                    {
                        if (string.IsNullOrEmpty(args.PropertyName) || args.PropertyName == intermediatePropertyName)
                        {
                            var newIntermediate = intermediateSelector(vm);

                            // If the intermediate changed, update subscription
                            ObserveIntermediateProperty(ctrl, newIntermediate);
                        }
                    };

                    vm.PropertyChanged += rootHandler;
                    subscriptions["Root"] = rootHandler;
                }
                else if (prop == intermediatePropertyName)
                {
                    // The intermediate property changed on the ViewModel
                    var newIntermediate = intermediateSelector(vm);
                    ObserveIntermediateProperty(ctrl, newIntermediate);
                }
                else if (prop == propertyName)
                {
                    // The specific property we're watching changed
                    var intermediate = intermediateSelector(vm);
                    if (intermediate != null)
                    {
                        try
                        {
                            TProperty value = propertySelector(intermediate);
                            InvokeCallback(ctrl, value);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine($"ObserveNestedProperty: Error accessing property after change: {ex.Message}");
                        }
                    }
                    else
                    {
                        InvokeCallback(ctrl, defaultValue);
                    }
                }
            }, debugTypeMismatch);

            // Set up cleanup
            control.ExecuteUponDisposal[mainKey] = () =>
            {
                foreach (var subscription in subscriptions)
                {
                    if (subscription.Key == "Root" && control.BindingContext is TSource sourceT)
                    {
                        sourceT.PropertyChanged -= subscription.Value;
                    }
                    else if (subscription.Key.StartsWith("Intermediate_"))
                    {
                        // Extract the hash code from the key
                        if (int.TryParse(subscription.Key.Substring("Intermediate_".Length), out int hashCode))
                        {
                            if (control.BindingContext is TSource sourceVm)
                            {
                                var intermediate = intermediateSelector(sourceVm);
                                if (intermediate != null && intermediate.GetHashCode() == hashCode)
                                {
                                    intermediate.PropertyChanged -= subscription.Value;
                                }
                            }
                        }
                    }
                }
                subscriptions.Clear();
            };

            return control;
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

        #region TEXT

        /// <summary>
        /// Sets the label font size in points (double)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="view"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static T SetFontSize<T>(this T view, double size) where T : SkiaLabel
        {
            view.FontSize = size;
            return view;
        }

        #endregion

        #region LAYOUT

        /// <summary>
        /// Sets the height of a SkiaControl to a specified size and returns the modified control.
        /// </summary>
        /// <typeparam name="T">Represents a control that can be modified to set its height.</typeparam>
        /// <param name="view">The control whose height is being set.</param>
        /// <param name="size">The new height value to be applied to the control.</param>
        /// <returns>The modified control with the updated height.</returns>
        public static T SetHeight<T>(this T view, double size) where T : SkiaControl
        {
            view.HeightRequest = size;
            return view;
        }


        /// <summary>
        /// Sets the width of a SkiaControl to a specified size and returns the modified control.
        /// </summary>
        /// <typeparam name="T">Represents a type that extends SkiaControl, allowing for width adjustments.</typeparam>
        /// <param name="view">The control whose width is being set to a new value.</param>
        /// <param name="size">The new width value to be applied to the control.</param>
        /// <returns>The modified control with the updated width.</returns>
        public static T SetWidth<T>(this T view, double size) where T : SkiaControl
        {
            view.WidthRequest = size;
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
        /// Fills horizontally
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="view"></param>
        /// <returns></returns>
        public static T FillX<T>(this T view) where T : SkiaControl
        {
            view.HorizontalOptions = LayoutOptions.Fill;
            return view;
        }

        public static T EndX<T>(this T view) where T : SkiaControl
        {
            view.HorizontalOptions = LayoutOptions.End;
            return view;
        }
        

        /// <summary>
        /// Fills vertically
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="view"></param>
        /// <returns></returns>
        public static T FillY<T>(this T view) where T : SkiaControl
        {
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

        #endregion

        /// <summary>
        /// Sets the margin for the control
        /// </summary>
        /// <typeparam name="T">Type of SkiaControl</typeparam>
        /// <param name="view">The control to set margin for</param>
        /// <param name="uniformMargin">The uniform margin to apply to all sides</param>
        /// <returns>The control for chaining</returns>
        public static T SetMargin<T>(this T view, double uniformMargin) where T : SkiaControl
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
        public static T SetMargin<T>(this T view, double horizontal, double vertical) where T : SkiaControl
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
        public static T SetMargin<T>(this T view, double left, double top, double right, double bottom)
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
