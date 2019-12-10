using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using TheXDS.MCART.ViewModel;
using TheXDS.Triton.Pages;

namespace TheXDS.Triton.ViewModels
{
    public class MainWindowViewModel : PageContainerViewModel
    {
        //public IEnumerable<TabHost> Pages
        //{
        //    get
        //    {
        //        yield return new TestPage();
        //        yield return new TestPage();
        //        yield return new TestPage();
        //    }
        //}
    }

    public class TabPageBuilder : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new TabHost(value as Page ?? throw new InvalidCastException());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException();
        }
    }






    public class TestViewModel : PageViewModel
    {
        private static int _count;

        public TestViewModel()
        {
            _count++;
            Title = $"Prueba # {_count}";
            AccentColor = TheXDS.MCART.Resources.Colors.Pick();
        }
    }
}
