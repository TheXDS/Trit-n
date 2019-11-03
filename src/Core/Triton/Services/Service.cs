using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Services
{




    public class Service<T> : IService where T : DbContext, new()
    {
        public Service(IServiceConfiguration settings)
        {
            if (settings is null) throw new ArgumentNullException(nameof(settings));

            ActiveSettings = settings;
        }

        public IServiceConfiguration ActiveSettings { get; }

        public ICrudFullTransaction GetFullTransaction()
        {
            return ActiveSettings.CrudTransactionFactory.ManufactureTransaction<T>(ActiveSettings);
        }
    }
}