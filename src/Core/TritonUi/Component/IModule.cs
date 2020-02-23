using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART.UI;
using TheXDS.Triton.Ui.ViewModels;

namespace TheXDS.Triton.Ui.Component
{
    public interface IModule
    {
        IGrouping<string, Launcher> Pages { get; }
    }
}