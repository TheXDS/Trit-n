using System;
using System.Collections.Generic;
using Gtk;
using TheXDS.MCART.Events;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Ui.Component;
using TheXDS.Triton.Ui.ViewModels;
using UI = Gtk.Builder.ObjectAttribute;

namespace TheXDS.Triton.Workstation.GtkSharpClient.Pages
{ 
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que permita 
    /// establecer un contexto de datos para utilizar internamente.
    /// </summary>
    /// <remarks>
    /// Para C#9, esta interfaz es candidato a mudarse a la característica de
    /// "shapes".
    /// </remarks>
    public interface IDataContext
    {
        /// <summary>
        /// Obtiene o establece el contexto de datos utilizado por esta
        /// instancia.
        /// </summary>
        object? DataContext { get; set; }
    }

    public abstract class GtkTritonPage : Bin, IDataContext
    {
        protected Builder Builder { get; }
        public object? DataContext { get; set; }

        public PageViewModel ViewModel { get; private set; }
        
        private GtkTritonPage(Builder builder, string id)  : base(builder.GetObject(id).Handle)
        {
            Builder = builder;
        }

        protected GtkTritonPage(string id) : this(new Builder($"{id}.glade"), id) { }

        protected void InitializeComponent()
        {
             Builder.Autoconnect(this);
        }

        internal void SetViewModel(PageViewModel vm)
        {
            ViewModel = vm;
        }
    }
    
    public class TestPage : GtkTritonPage
    {
        [UI] private Label lblPageTitle = null!;
        [UI] private Entry txtName = null!;
        [UI] private Label lblGreet = null!;
        [UI] private SpinButton nudOne = null!;
        [UI] private SpinButton nudTwo = null!;
        [UI] private Button btnAdd = null!;
        [UI] private Label lblResult = null!;
        [UI] private Button btnQuit = null!;

        public TestPage() : base(nameof(TestPage))
        {
            InitializeComponent();
            btnQuit.Bind(ViewModel.CloseCommand);
        }
    }
}