using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawnUi.Maui.Internals
{


    public class TitleWithStringId
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }

    public class SelectableAction : TitleWithStringId, ISelectableOption
    {
        public SelectableAction()
        {
            Id = Guid.NewGuid().ToString();
        }
        public SelectableAction(string title, Action action)
        {
            Id = Guid.NewGuid().ToString();
            Title = title;
            Action = action;
        }

        public SelectableAction(string id, string title, Action action)
        {
            Id = id;
            Title = title;
            Action = action;
        }


        public Action Action { get; set; }
        public bool Selected { get; set; }

        public bool IsReadOnly { get; } = false;
    }
}
