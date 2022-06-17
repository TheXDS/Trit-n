using System.Data;

namespace TheXDS.Triton.Dapper.Services;

/// <summary>
/// Define una serie de miembros a implementar por un tipo que permita generar y abrir conexiones a bases de datos.
/// </summary>
public interface IDbConnectionFactory
{
    /// <summary>
    /// Abre una nueva conexión a una base de datos.
    /// </summary>
    /// <returns></returns>
    IDbConnection OpenConnection();
}
