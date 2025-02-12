using System.Collections.Generic;
using System.Threading.Tasks;
using AppoMobi.Specials;
using DrawnUi.Maui.Draw;
using TestCalendar.Views;

namespace AppoMobi.Xam
{
    public class OptionsList<T> : List<T> where T : ISelectableOption
    {
	    public OptionsList(List<T> list) : base(list)
	    {
	    }

	    public OptionsList() 
	    {
	    }

		public T GetById(string id)
        {
            if (this.Any())
            {
                return this.FirstOrDefault(x => x.Id == id);
            }
            return default(T);
        }

        
        public bool SelectOne(string id)
        {
            var found = this.FirstOrDefault(x => x.Id == id);
            if (found == null) return false;
            SelectOne(found);
            return true;
        }
        
        public T Selected
        
        {
            get
            {
                return this.FirstOrDefault(x => x.Selected);
            }
        }
        
        public IEnumerable<T> SelectedMany
        
        {
            get
            {
                return this.Where(x => x.Selected);
            }
        }
        
        public void SelectOne(T item)
        
        {
            foreach (var member in this)
            {
                if (member.Equals(item))
                {
                    member.Selected = true;
                }
                else
                {
                    member.Selected = false;
                }
            }
        }

    }
    public class OptionsList
    {
	    readonly bool _closeOnChildTapped;

        public OptionsList(string title, List<ISelectableOption> list, string cancel = "Cancel", bool closeOnChildTapped=false)
        {
            sMessage = title;
            sNo = cancel;
            Items = list;
            _closeOnChildTapped = closeOnChildTapped;
        }



        protected List<ISelectableOption> Items { get; set; }
        bool bDisableBackgroundClick { get; set; }
        bool bQuitOnBackPressed { get; set; }

        public void DisableBackgroundClick(bool value = true)
        {
            bDisableBackgroundClick = value;
        }

        public void QuitOnBackPressed(bool value = true)
        {
            bQuitOnBackPressed = value;
        }

        void CallbackAfter(int ret)
        
        {
            bFinished = true;
        }

        public string sNo { get; set; }
        public string sMessage { get; set; }
        public bool bFinished { get; set; } = false;

        /*
        public async Task ShowAsync(bool showNo = true)
        {
            var popup = new PopupOptionsListView(CallbackAfter, sMessage, Items, sNo, bDisableBackgroundClick,
                bQuitOnBackPressed);
            popup.CloseOnChildTapped = _closeOnChildTapped;
            await PopupNavigation.Instance.PushAsync(popup);
            while (!bFinished)
            {
                await Task.Delay(100);
            }
            return;
        }
        */


    }
 
}
