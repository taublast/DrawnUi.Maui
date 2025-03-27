using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Components;

namespace DrawnUi.Views
{
    public static class XamlExtensions
    {

    }

    public class VisualElement : LayoutComponentBase, IDisposable, INotifyPropertyChanged
    {
        
        [Parameter]
        public string Units { get; set; } = "px";

        protected override void OnParametersSet()
        {
            var cssForGrid = "";
            var col = 0;
            var row = 0;
            if (GridColumn != null)
            {
                cssForGrid += $" grid-column-start: {GridColumn+1};";
                col = GridColumn.Value;
            }
            if (GridRow != null)
            {
                cssForGrid += $" grid-row-start: {GridRow+1};";
                row = GridRow.Value;
            }
            if (GridColumnSpan != null)
            {
                cssForGrid += $" grid-column-end: {col+GridColumnSpan+1};";
            }
            if (GridRowSpan != null)
            {
                cssForGrid += $" grid-row-end: {row + GridRowSpan+1};";
            }

            CssGridPosition = cssForGrid;

            if (!string.IsNullOrEmpty(Margin))
            {
                var cols = Margin.Split(',', StringSplitOptions.RemoveEmptyEntries);
                var nbCol = -1;

                if (cols.Length == 1)
                {
                    CssMargins = $"margin: {cols[0].Trim()}{Units};";
                }
                else if (cols.Length == 2)
                {
                    CssMargins = $"margin: {cols[1].Trim()}{Units} {cols[0].Trim()}{Units};";
                }
                else if (cols.Length == 4)
                {
                    // L T R B -> T R B L
                    // 0 1 2 3 -> 1 2 3 0
                    CssMargins =
                        $"margin: {cols[1].Trim()}{Units} {cols[2].Trim()}{Units} {cols[3].Trim()}{Units} {cols[0].Trim()}{Units};";
                }
            }

            if (!string.IsNullOrEmpty(Padding))
                {
                    var cols = Padding.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    var nbCol = -1;

                    if (cols.Length == 1)
                    {
                        var value = $"{cols[0].Trim()}";
                        CssPadding = $"padding: {value}{Units};";
                        PaddingParsed = new Thickness(value.ToDouble());
                    }
                    else if (cols.Length == 2)
                    {
                        CssPadding = $"padding: {cols[1].Trim()}{Units} {cols[0].Trim()}{Units};";
                    }
                    else if (cols.Length == 4)
                    {
                        // L T R B -> T R B L
                        // 0 1 2 3 -> 1 2 3 0
                        CssPadding = $"padding: {cols[1].Trim()}{Units} {cols[2].Trim()}{Units} {cols[3].Trim()}{Units} {cols[0].Trim()}{Units};";
                    }
                }

            base.OnParametersSet();
        }

        public Thickness PaddingParsed { get; set; }

        protected string CssMargins { get; set; }

        protected string CssPadding { get; set; }

        protected string CssGridPosition { get; set; }

        [Parameter]
        public int? GridColumnSpan { get; set; }

        [Parameter]
        public int? GridRowSpan { get; set; }

        [Parameter]
        public int? GridColumn { get; set; }

        [Parameter]
        public int? GridRow { get; set; }

        [Parameter]
        public LayoutAlignment HorizontalOptions { get; set; } = LayoutAlignment.Start;

        [Parameter]
        public LayoutAlignment VerticalOptions { get; set; } = LayoutAlignment.Start;

        //[Parameter]
        //public bool HorizontalOptionsExpands { get; set; }  

        //[Parameter]
        //public bool VerticalOptionsExpands { get; set; }  

        [Parameter] 
        public double WidthRequest { get; set; } = -1.0;

        [Parameter]
        public double HeightRequest { get; set; } = -1.0;

        protected string CssLayoutAlignment
        {
            get
            {
                var ret = "";
                var width = " width: fit-content;"; //for layout.start
                var height = " height: fit-content;"; //for layout.start

                //if (HorizontalOptionsExpands)
                //    width = "";

                //if (VerticalOptionsExpands)
                //    height = "";


                if (WidthRequest >= 0)
                {
                    width = $"width: {WidthRequest}{Units};".Replace(",",".");
                }
                else
                {
                    if (HorizontalOptions == LayoutAlignment.Fill)
                    {
                        width = $"width: initial;";
                    }
                }

                if (HeightRequest >= 0)
                {
                    height = $"height: {HeightRequest}{Units};".Replace(",", "."); ;
                }
                else
                {
                    if (VerticalOptions == LayoutAlignment.Fill)
                    {
                        height = $"height: 100%;";
                    }
                }

                if (HorizontalOptions == LayoutAlignment.Start || HorizontalOptions == LayoutAlignment.Fill)
                {
                    ret +=  width;
                }
                else
                if (HorizontalOptions == LayoutAlignment.Center)
                {
                    ret += "margin-left: auto; margin-right: auto;"+width;
                }
                else
                if (HorizontalOptions == LayoutAlignment.End)
                {
                    ret += "margin-left: auto;"+width;
                }

                if (VerticalOptions == LayoutAlignment.Start || VerticalOptions == LayoutAlignment.Fill)
                {
                    ret += height;
                }
                else
                if (VerticalOptions == LayoutAlignment.Center)
                {
                    ret += "margin-top: auto; margin-bottom: auto;"+height;
                 //ret += "top: 50%; transform: translate(0, -50%); " + height; 
                }
                else
                if (VerticalOptions == LayoutAlignment.End)
                {
                    ret += "margin-top: auto;"+height;
                }

                return ret;
            }
        }


        [Parameter]
        public string Margin { get; set; }

        [Parameter]
        public string Padding { get; set; }

        [Parameter]
        public string Class { get; set; }

        [Parameter]
        public string Style { get; set; }

        [Parameter]
        public bool IsVisible { get; set; } = true;

        protected override bool ShouldRender()
        {
            return IsVisible && base.ShouldRender();
        }

        public bool Not(bool value)
        {
            return !value;
        }

        public string Uid { get; protected set; } = Guid.NewGuid().ToString();


        //protected string AdjustPositionOpen
        //{
        //    get
        //    {
        //        if (!string.IsNullOrEmpty(CssMargins))
        //            return $"<div style='{CssMargins}'>";

        //        return string.Empty; ;
        //    }
        //}

        //protected string AdjustPositionClose
        //{
        //    get
        //    {
        //        if (!string.IsNullOrEmpty(CssMargins))
        //            return CloseDiv;

        //        return string.Empty; 
        //    }
        //}

        public async void Update()
        {
            await InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
#if DEBUG
            //       dynamic value = Reflection.GetPropertyValueFor(this, propertyName);
            //       Console.WriteLine($"[PropertyChanged] BasePage {propertyName} = {value}");
#endif
             
            Update();

            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
        #endregion

        public virtual void OnDisposing()
        {
            //todo

        }

        public void Dispose()
        {
            OnDisposing();
        }

        /// <summary>
        /// Use to get class unique class name for use in markup
        /// </summary>
        public MarkupString ClassUid
        {
            get
            {
                return new MarkupString($"xe{Uid.Replace("-", "")}");
            }
        }

        /// <summary>
        /// Use to get class unique class name for use in stylesheet with preceding dot
        /// </summary>
        public MarkupString ClassUidStyle
        {
            get
            {
                return new MarkupString($".xe{Uid.Replace("-", "")}");
            }
        }

    }
}
