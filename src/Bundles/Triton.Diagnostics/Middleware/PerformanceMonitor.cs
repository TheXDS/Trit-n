using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using TheXDS.MCART.Events;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services;

namespace TheXDS.Triton.Middleware
{
    /// <summary>
    /// Middleware que permite obtener información específica sobre el
    /// tiempo que toma ejecutar acciones Crud.
    /// </summary>
    public class PerformanceMonitor : INotifyPropertyChanged, ITransactionMiddleware
    {
        private readonly List<double> _events = new List<double>();
        private readonly Stopwatch _stopwatch = new Stopwatch();
        
        /// <summary>
        /// Ocurre cuando se ha producido la acción Crud
        /// <see cref="CrudAction.Commit"/>.
        /// </summary>
        public event EventHandler<ValueEventArgs<double>>? Elapsed;

        /// <summary>
        /// Ocurre cuando el valor de una propiedad ha cambiado.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Obtiene la cantidad de eventos de guardado registrados por esta
        /// instancia.
        /// </summary>
        public int EventCount => _events.Count;

        /// <summary>
        /// Obtiene el tiempo promedio en milisegundos que han tomado las
        /// operaciones de guardado.
        /// </summary>
        public double AverageMs => Get(Enumerable.Average);

        /// <summary>
        /// Obtiene la cantidad de tiempo en milisegundos que ha tomado la
        /// operación de guardado más corta.
        /// </summary>
        public double MinMs => Get(Enumerable.Min);

        /// <summary>
        /// Obtiene la cantidad de tiempo en milisegundos que ha tomado la
        /// operación de guardado más larga.
        /// </summary>
        public double MaxMs => Get(Enumerable.Max);

        /// <summary>
        /// Reinicia los contadores de desempeño de esta instancia.
        /// </summary>
        public void Reset() => _events.Clear();

        private double Get(Func<List<double>, double> func)
        {
            return _events.Any() ? func(_events) : double.NaN;
        }

        internal ServiceResult? BeforeAction(CrudAction arg1, Model? _)
        {
            if (arg1.HasFlag(CrudAction.Commit)) _stopwatch.Restart();
            return null;
        }

        internal ServiceResult? AfterAction(CrudAction arg1, Model? _)
        {
            if (arg1.HasFlag(CrudAction.Commit))
            {
                _stopwatch.Stop();
                Elapsed?.Invoke(this, _stopwatch.Elapsed.TotalMilliseconds.PushInto(_events));

                Notify(nameof(EventCount));
                Notify(nameof(AverageMs));
                Notify(nameof(MinMs));
                Notify(nameof(MaxMs));
            }
            return null;
        }

        private void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}