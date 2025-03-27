using System;
using Microsoft.AspNetCore.Components;

namespace DrawnUi.Views
{
    public class ViewCell<T> : VisualElement
    {
        [Parameter]
        public T BindingContext { get; set; }

        [Parameter]
        public Action<T> OnSelected { get; set; }

        [Parameter]
        public EventCallback<T> BindingContextChanged { get; set; }

    }

    //
    // Summary:
    //     The orientations the a StackLayout can have.
}