using System;
using System.Collections.Generic;
using Gtk;
using TheXDS.MCART.Events;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Ui.Component;
using TheXDS.Triton.Ui.ViewModels;
using TheXDS.Triton.Workstation.GtkSharpClient.Pages;
using UI = Gtk.Builder.ObjectAttribute;

namespace TheXDS.Triton.Workstation.GtkSharpClient
{
    internal class MainWindow : Window
    {
        private HostViewModel _viewModel = new HostViewModel();

        [UI] private Notebook _tabRoot = null!;

        public MainWindow() : this(new Builder($"{nameof(MainWindow)}.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetObject(nameof(MainWindow)).Handle)
        {
            builder.Autoconnect(this);
            DeleteEvent += (_, e) => Application.Quit();
            
            _viewModel.PageAdded += AddVisualPage;
            _viewModel.PageClosed += CloseVisualPage;

            var r = new DictionaryVisualResolver<Bin>();
            r.RegisterVisual<TestViewModel, TestPage>();
            
            _viewModel.AddPage(new TestViewModel());
            _viewModel.AddPage(new TestViewModel());
            _viewModel.AddPage(new TestViewModel());
        }

        private void CloseVisualPage(object? sender, ValueEventArgs<PageViewModel> e)
        {
            _tabRoot.RemovePage(_openPages[e.Value]);
            _openPages.Remove(e.Value);
        }

        private Dictionary<PageViewModel,int> _openPages = new Dictionary<PageViewModel, int>();
        
        private void AddVisualPage(object? sender, ValueEventArgs<PageViewModel> e)
        {
            var s = new Label($"Placeholder para el contenido de {e.Value.Title}. Color: {e.Value.AccentColor}"); 
            _openPages.Add(e.Value, _tabRoot.Page = _tabRoot.AppendPage(s, new Label(e.Value.Title)));
            s.Show();
        }
    }
    
    public class TestFvr : FallbackVisualResolver<Widget>
    {
        public TestFvr(IVisualResolver<Widget> resolver) : base(resolver)
        {
        }

        /// <inheritdoc/>
        protected override Widget FallbackResolve(PageViewModel viewModel, Exception ex)
        {
            return new FallbackErrorPage("Error al resolver la página solicitada",$"{ex.GetType().Name}{ex.Message.OrNull(": {0}")}");
        }
    }
}
