using System;
using System.Collections.Generic;
using System.Text;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Middleware
{
    public interface ITransactionMiddleware
    {
        ServiceResult? BeforeAction(CrudAction arg1, Model? arg2) => null;
        ServiceResult? AfterAction(CrudAction arg1, Model? arg2) => null;
    }
}
