using TheXDS.Triton.ViewModels;
using System.Collections.Generic;
using System;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Triton.Component
{
    /// <summary>
    ///     Implementa un <see cref="IVisualResolver"/> que contiene un
    ///     diccionario que mapea un <see cref="PageViewModel"/> por su tipo
    ///     con un contenedor visual de tipo <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    ///     Tipo de contenedor visual a implementar.
    /// </typeparam>
    public class DictionaryVisualResolver<T> : IVisualResolver<T> where T : new()
    {
        private readonly Dictionary<Type, Type> _mappings = new Dictionary<Type, Type>();

        /// <summary>
        ///     Resuelve el contenedor visual a utilizar para alojar al 
        ///     <see cref="PageViewModel"/> especificado.
        /// </summary>
        /// <param name="viewModel">
        ///     <see cref="PageViewModel"/> que va a alojarse.
        /// </param>
        /// <returns>
        ///     Un contenedor visual fuertemente tipeado para el
        ///     <see cref="PageViewModel"/> especificado.
        /// </returns>
        public T ResolveVisual(PageViewModel viewModel)
        {
            return _mappings[viewModel.GetType()].New<T>();
        }

        /// <summary>
        ///     Mapea el tipo de <see cref="PageViewModel"/> con un tipo de
        ///     contenedor visual a resolver.
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <typeparam name="TVisual"></typeparam>
        public void RegisterVisual<TViewModel, TVisual>() where TViewModel : PageViewModel where TVisual : T, new()
        {
            _mappings.Add(typeof(TViewModel), typeof(TVisual));
        }
    }
}