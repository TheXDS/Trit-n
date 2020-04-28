using System.Windows.Controls;
using TheXDS.Triton.Pages;
using TheXDS.Triton.Ui.Component;
using TheXDS.Triton.Ui.ViewModels;

namespace TheXDS.Triton.WpfClient.ViewModels
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