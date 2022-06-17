namespace TheXDS.Triton.Dapper.Services;

/// <summary>
/// Contiene información sobre las invalidaciones de metadatos a aplicar a un
/// modelo de datos en concreto.
/// </summary>
/// <param name="TableName">Nombre de tabla a utilizar al construir las
/// sentencias SQL requeridas.</param>
/// <param name="Properties">
/// Diccionario de invalidaciones para las propiedades del modelo de datos.
/// </param>
public record struct DapperModelDescriptor(string TableName, IDictionary<string, string>? Properties);
