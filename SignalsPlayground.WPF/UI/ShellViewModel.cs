using System;
using System.Collections.Generic;
using System.Linq;
using SignalsPlayground.WPF.UI.Pages;
using Stylet;

namespace SignalsPlayground.WPF.UI
{
    public class ShellViewModel : Conductor<NavigationPageViewModel>.Collection.OneActive
    {
        private BindableCollection<NavigationPageViewModel> _pages = new BindableCollection<NavigationPageViewModel>();
        public BindableCollection<NavigationPageViewModel> Pages
        {
            get => _pages;
            set => SetAndNotify(ref _pages, value);
        }

        public ShellViewModel(DaubechiesWaveletPageViewModel daubechiesVM)
        {
            Pages.Add(daubechiesVM);
            ActiveItem = Pages.First();
        }
    }
}
