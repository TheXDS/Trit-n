using TheXDS.MCART;
using TheXDS.MCART.Exceptions;
using TheXDS.Triton.ViewModels;

namespace TheXDS.Triton.Component
{
    /// <summary>
    ///     Define una serie de métodos a implementar por una clase que permita
    ///     cerrar un contenedor visual o un elemento de UI.
    /// </summary>
    public interface ICloseable
    {
        /// <summary>
        ///     Cierra el elemento activo.
        /// </summary>
        void Close();
    }

    public abstract class UiBuilder
    {
        public static UiBuilder Builder { get; } = Objects.FindFirstObject<UiBuilder>() ?? throw new MissingTypeException(typeof(UiBuilder));

        public abstract ICloseable BuildHost(PageViewModel viewModel);

    }
}