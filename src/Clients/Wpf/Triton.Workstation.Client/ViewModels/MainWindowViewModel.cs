using System;
using System.Windows.Controls;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Pages;
using TheXDS.Triton.Ui.Component;
using TheXDS.Triton.Ui.ViewModels;
using Triton.Workstation.Client.Pages;

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
            return new TabBuilder(new TestFvr(r));
        }
    }

    public class TestFvr : FallbackVisualResolver<Page>
    {
        public TestFvr(IVisualResolver<Page> resolver) : base(resolver)
        {
        }

        protected override Page FallbackResolve(Exception ex)
        {
            return new FallbackErrorPage { Message = $"{ex.GetType().Name}{ex.Message.OrNull(": {0}")}" };
        }
    }
}