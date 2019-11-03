using System;
using System.Collections.Generic;
using System.Text;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Security.Base
{
    interface ISecurityDevice
    {
        bool CanRun(CrudAction action);
    }
}
