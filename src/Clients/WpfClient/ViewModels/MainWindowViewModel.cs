using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TheXDS.MCART.ViewModel;
using TheXDS.Triton.Component;
using TheXDS.Triton.Pages;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.ViewModels;

namespace TheXDS.Triton.ViewModels
{
    public class MainWindowViewModel : HostViewModel<TabHost>
    {
        public MainWindowViewModel() : base(CreateBuilder())
        {
            AddPage(new TestViewModel());
            AddPage(new TestViewModel());
            AddPage(new TestViewModel());

        }


        private static IVisualBuilder<TabHost> CreateBuilder()
        {
            var r = new DictionaryVisualResolver<Page>();
            r.RegisterVisual<TestViewModel, TestPage>();
            return new TabBuilder(r);
        }

    }

    public class TabBuilder : IVisualBuilder<TabHost>
    {
        private readonly IVisualResolver<Page> _resolver;
        
        public TabBuilder(IVisualResolver<Page> resolver)
        {
            _resolver = resolver;
        }
        
        public TabHost Build(PageViewModel viewModel)
        {
            return new TabHost(viewModel, _resolver.ResolveVisual(viewModel));
        }
    }
}