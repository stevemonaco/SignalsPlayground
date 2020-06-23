using Stylet;
using System;
using System.Collections.Generic;
using System.Text;

namespace SignalsPlayground.WPF.UI
{
    public class NavigationPageViewModel : Screen
    {
        private string _pageName;
        public string PageName
        {
            get => _pageName;
            set => SetAndNotify(ref _pageName, value);
        }
    }
}
