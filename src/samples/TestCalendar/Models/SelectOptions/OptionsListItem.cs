using System;
using AppoMobi.Models;
 

namespace AppoMobi.Xam
{
    public class OptionsListItem : ObservableObject
    {
        public OptionsListItem()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }
        public string Tag { get; set; }
        public string Help { get; set; }

        public bool IsSwitch { get; set; }
        public bool SwitchValue { get; set; }
        public string SelectionDesc { get; set; }
        public string ActionDesc { get; set; }
        
        public Func<bool> IsVisible;        

        public Action <bool>Switched;

    }



}
