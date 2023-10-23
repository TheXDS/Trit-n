using TheXDS.Triton.Models.Base;

namespace TheXDS.Triton.InMemory.Services;

/// <summary>
/// Fábrica de transacciones que genera transacciones sin persistencia (en
/// memoria).
/// </summary>
public class InMemoryTransFactory : CollectionTransFactory
{
    private static readonly List<Model> _store = new();

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="InMemoryTransFactory"/>.
    /// </summary>
    public InMemoryTransFactory() : base(_store)
    {
    }

    /// <summary>
    /// Limpia la base de datos en memoria.
    /// </summary>
    public static void Wipe()
    {
        _store.Clear();
    }
}
