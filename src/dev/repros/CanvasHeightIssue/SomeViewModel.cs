using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppoMobi.Specials;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Sandbox
{
    public partial class SomeViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableRangeCollection<int> items = [1, 2];

        [RelayCommand]
        public async Task UpdateItemsAsync()
        {
            this.Items.AddRange([2, 3, 4, 5]);

            await Task.Delay(TimeSpan.FromSeconds(3));

            this.Items.ReplaceRange(Enumerable.Range(0, 100).ToList());
        }
    }
}
