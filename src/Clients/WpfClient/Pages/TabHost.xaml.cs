using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheXDS.Triton.Component;
using TheXDS.Triton.ViewModels;

namespace TheXDS.Triton.Pages
{
    /// <summary>
    /// Lógica de interacción para TabHost.xaml
    /// </summary>
    public partial class TabHost : TabItem, IVisualHost
    {
        public TabHost()
        {
            InitializeComponent();
            DataContext = new TabHostViewModel(content, this)
            {
                AccentColor = MCART.Resources.Colors.Pick() 
            };
            content.DataContext = DataContext;
        }

        public void Activate()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public static implicit operator TabHost(Page page)
        {
            return new TabHost(page);
        }
    }
}
