using Microsoft.EntityFrameworkCore;
using TheXDS.MCART.Types.Base;

namespace TheXDS.Triton.Services.Base
{
    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que permita
    /// realizar operaciones de lectura y de escritura basadas en
    /// transacción sobre una base de datos.
    /// </summary>
    public interface ICrudReadWriteTransaction : ICrudReadTransaction, ICrudWriteTransaction
    {
    }

    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que permita
    /// realizar operaciones de lectura y de escritura basadas en
    /// transacción sobre una base de datos.
    /// </summary>
    public interface ICrudReadWriteTransaction<TContext> : ICrudReadWriteTransaction where TContext : DbContext , new() 
    {
        /// <summary>
        /// Obtiene a la instancia de contexto activa en esta transacción.
        /// </summary>
        TContext Context { get; }
    }
}
