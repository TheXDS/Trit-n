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
    public class FallbackErrorPage : Bin
    {
        [UI] private Label _lblTitle = null!;
        [UI] private Label _lblMessage = null!;
        
        public FallbackErrorPage(string title, string message) : this(new Builder($"{nameof(FallbackErrorPage)}.glade"))
        {
            _lblTitle.Text = title;
            _lblMessage.Text = title;
            ShowAll();
        }
        
        private FallbackErrorPage(Builder builder) : base(builder.GetObject(nameof(FallbackErrorPage)).Handle)
        {
            builder.Autoconnect(this);
        }
    }
}