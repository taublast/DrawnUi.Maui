using System.Windows.Input;

namespace DrawnUi.Maui.Draw
{
    /// <summary>
    /// Provides extension methods for fluent API design pattern with DrawnUI controls
    /// </summary>
    public static partial class FluentExtensions
    {
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
        public static T WithMargin<T>(this T view, double left, double top, double right, double bottom) where T : SkiaControl
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
        public static T WithPadding<T>(this T view, double left, double top, double right, double bottom) where T : SkiaLayout
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


    }
}
