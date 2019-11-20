using System;
using System.Collections.Generic;
using System.Text;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Security.Base
{
    public interface ICrudSecurityDevice
    {
        bool CanRun(CrudAction action);
    }

    public static class SecurityServices
    {
        public static TransactionConfiguration UseBasicSecurity(this TransactionConfiguration config)
        {

            return config;
        }
    }
}
