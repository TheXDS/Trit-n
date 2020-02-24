using System.Collections.Generic;
using System.Linq;
using TheXDS.MCART.Types.Base;
using TheXDS.MCART.UI;

namespace TheXDS.Triton.Ui.Component
{
    public interface IModule : INameable
    {
        IGrouping<string, Launcher> Launchers { get; }
    }
}