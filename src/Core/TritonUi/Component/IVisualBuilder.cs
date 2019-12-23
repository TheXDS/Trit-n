using TheXDS.Triton.ViewModels;

namespace TheXDS.Triton.Component
{
    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que permita
    ///     construir hosts visuales para un <see cref="PageViewModel"/>.
    /// </summary>
    public interface IVisualBuilder
    {
        /// <summary>
        ///     Construye un host visual para el <see cref="PageViewModel"/>
        ///     especificado.
        /// </summary>
        /// <param name="viewModel">
        ///     <see cref="PageViewModel"/> para el cual construir un host
        ///     visual.
        /// </param>
        /// <returns>
        ///     Un host visual para el <see cref="PageViewModel"/>
        ///     especificado.
        /// </returns>
        ICloseable Build(PageViewModel viewModel);
    }

    /// <summary>
    ///     Define una serie de miembros a implementar por un tipo que permita
    ///     construir hosts visuales para un <see cref="PageViewModel"/>.
    /// </summary>
    public interface IVisualBuilder<T> : IVisualBuilder where T : ICloseable
    {
        /// <summary>
        ///     Construye un host visual para el <see cref="PageViewModel"/>
        ///     especificado.
        /// </summary>
        /// <param name="viewModel">
        ///     <see cref="PageViewModel"/> para el cual construir un host
        ///     visual.
        /// </param>
        /// <returns>
        ///     Un host visual para el <see cref="PageViewModel"/>
        ///     especificado.
        /// </returns>
        new T Build(PageViewModel viewModel);

        ICloseable IVisualBuilder.Build(PageViewModel viewModel) => Build(viewModel);
    }
}