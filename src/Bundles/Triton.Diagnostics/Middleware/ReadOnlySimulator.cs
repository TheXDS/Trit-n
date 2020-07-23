﻿using System;
using TheXDS.MCART.Exceptions;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Middleware
{
    /// <summary>
    /// Middleware estático que bloquea todas las operaciones de escritura
    /// de datos, devolviendo para las mismas siempre el resultado
    /// <see cref="ServiceResult.Ok"/> o un error producido en el epílogo
    /// de la transacción.
    /// </summary>
    public static class ReadOnlySimulator
    {
        private static TransactionConfiguration? _config;

        /// <summary>
        /// Configura la transacción para simular las operaciones sin realizar
        /// ninguna acción.
        /// </summary>
        /// <param name="config">
        /// Configuración de transacción sobre la cual aplicar.
        /// </param>
        /// <returns>
        /// La misma instancia que <paramref name="config"/>, permitiendo
        /// utilizar sintaxis Fluent.
        /// </returns>
        public static TransactionConfiguration UseSimulation(this TransactionConfiguration config)
        {
            if (_config is { }) throw new InvalidOperationException();                        
            return _config = config.AddLastProlog(SkipActualCall);
        }

        private static ServiceResult? SkipActualCall(CrudAction arg1, Model? arg2)
        {
            if (arg1 == CrudAction.Read) return null;
            return (_config ?? throw new TamperException()).Epilog(arg1, arg2) ?? ServiceResult.Ok;
        }
    }
}