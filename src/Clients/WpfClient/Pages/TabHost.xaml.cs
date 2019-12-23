using System;
using System.Windows.Controls;
using System.Windows.Navigation;
using TheXDS.Triton.Component;
using TheXDS.Triton.ViewModels;

namespace TheXDS.Triton.Pages
{
    /// <summary>
    /// Lógica de interacción para TabHost.xaml
    /// </summary>
    public partial class TabHost : TabItem, ICloseable
    {
        public TabHost(PageViewModel viewModel, Page content)
        {
            InitializeComponent();

            DataContext = viewModel;
            var f = new Frame()
            {
                NavigationUIVisibility = NavigationUIVisibility.Hidden
            };
            f.Navigate(content);
            Content = f;
            content.DataContext = DataContext;
        }

        public void Close()
        {
            throw new NotImplementedException();
        }
    }
}
