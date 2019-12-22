using System.Windows.Controls;
using TheXDS.Triton.Component;
using TheXDS.Triton.Pages;
using TheXDS.MCART.Resources;

namespace TheXDS.Triton.ViewModels
{
    public class TabHostViewModel<TPage> : PageViewModel<TabHost> where TPage : Page, new()
    {
        public TabHostViewModel()
        {
            Content = new TPage();
        }

        public Page Content { get; }
    }    
}