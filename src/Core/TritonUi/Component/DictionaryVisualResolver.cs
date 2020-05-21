using TheXDS.Triton.Ui.ViewModels;
using System.Collections.Generic;
using System;
using TheXDS.MCART.Types.Extensions;
using System.Diagnostics;

namespace TheXDS.Triton.Ui.Component
{
    /// <summary>
    /// Implementa un <see cref="IVisualResolver"/> que contiene un
    /// diccionario que mapea un <see cref="PageViewModel"/> por su tipo
    /// con un contenedor visual de tipo <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// Tipo de contenedor visual a implementar.
    /// </typeparam>
    public class DictionaryVisualResolver<T> : IVisualResolver<T> where T : new()
    {
        private readonly Dictionary<Type, Type> _mappings = new Dictionary<Type, Type>();

        /// <summary>
        /// Resuelve el contenedor visual a utilizar para alojar al 
        /// <see cref="PageViewModel"/> especificado.
        /// </summary>
        /// <param name="viewModel">
        /// <see cref="PageViewModel"/> que va a alojarse.
        /// </param>
        /// <returns>
        /// Un contenedor visual fuertemente tipeado para el
        /// <see cref="PageViewModel"/> especificado.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Se produce si se intenta resolver el contenedor visual para un
        /// valor nulo.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// Se produce si se intenta resolver el contenedor visual para un
        /// tipo de <see cref="PageViewModel"/> que no ha sido registrado.
        /// </exception>
        [DebuggerNonUserCode]
        public T ResolveVisual(PageViewModel viewModel)
        {
            return _mappings[viewModel.GetType()].New<T>();
        }

        /// <summary>
        /// Resuelve el contenedor visual a utilizar para alojar a un 
        /// <see cref="PageViewModel"/> del tipo especificado.
        /// </summary>
        /// <typeparam name="TViewModel">
        /// Tipo de <see cref="PageViewModel"/> que va a alojarse.
        /// </typeparam>
        /// <returns>
        /// Un contenedor visual fuertemente tipeado para el
        /// <see cref="PageViewModel"/> especificado.
        /// </returns>
        /// <exception cref="KeyNotFoundException">
        /// Se produce si se intenta resolver el contenedor visual para un
        /// tipo de <see cref="PageViewModel"/> que no ha sido registrado.
        /// </exception>
        [DebuggerNonUserCode]
        public T ResolveVisual<TViewModel>() where TViewModel : PageViewModel
        {
            return _mappings[typeof(TViewModel)].New<T>();
        }

        /// <summary>
        /// Registra la resolución de un <see cref="PageViewModel"/> a un tipo
        /// de contenedor visual a utilizar para presentarlo.
        /// </summary>
        /// <typeparam name="TViewModel">
        /// Tipo de <see cref="PageViewModel"/> que va a alojarse.
        /// </typeparam>
        /// <typeparam name="TVisual">
        /// Tipo de contenedor visual a utilizar para mostrar el <see cref="PageViewModel"/> a registrar.
        /// </typeparam>
        public void RegisterVisual<TViewModel, TVisual>() where TViewModel : PageViewModel where TVisual : T, new()
        {
            _mappings.Add(typeof(TViewModel), typeof(TVisual));
        }
    }
}