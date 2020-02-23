using TheXDS.MCART;
using TheXDS.MCART.Exceptions;
using TheXDS.Triton.Ui.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Triton.Ui.Component
{
    /// <summary>
    /// Define una serie de métodos a implementar por una clase que permita
    /// cerrar un contenedor visual o un elemento de UI.
    /// </summary>
    public interface ICloseable
    {
        /// <summary>
        /// Cierra el elemento activo.
        /// </summary>
        void Close();
    }












    public class TestViewModel : PageViewModel
    {
        private static int _count;
        private int _numberOne;
        private int _numberTwo;
        private int _result;

        public TestViewModel()
        {
            _count++;
            Title = $"Prueba # {_count}";
            AccentColor = MCART.Resources.Colors.Pick();
            SumCommand = new MCART.ViewModel.SimpleCommand(OnSum);
        }

        /// <summary>
        /// Obtiene o establece el valor NumberOne.
        /// </summary>
        /// <value>El valor de NumberOne.</value>
        public int NumberOne
        {
            get => _numberOne;
            set => Change(ref _numberOne, value);
        }

        /// <summary>
        /// Obtiene o establece el valor NumberTwo.
        /// </summary>
        /// <value>El valor de NumberTwo.</value>
        public int NumberTwo
        {
            get => _numberTwo;
            set => Change(ref _numberTwo, value);
        }

        /// <summary>
        /// Obtiene el comando relacionado a la acción Sum.
        /// </summary>
        /// <returns>El comando Sum.</returns>
        public System.Windows.Input.ICommand SumCommand { get; }

        private void OnSum()
        {
            Result = NumberOne + NumberTwo;
        }

        /// <summary>
        /// Obtiene o establece el valor Result.
        /// </summary>
        /// <value>El valor de Result.</value>
        public int Result
        {
            get => _result;
            private set => Change(ref _result, value);
        }
    }
}