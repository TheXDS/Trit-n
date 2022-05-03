using System;
using System.Collections.Generic;
using System.Linq;

namespace TheXDS.Triton.Middleware
{
    /// <summary>
    /// Contador de rendimiento que contiene una colección de muestras que
    /// puede utilizarse para generar gráficos.
    /// </summary>
    public class PlottablePerfMonitor : PerformanceMonitorBase
    {
        private readonly Queue<double> _events = new();
        private int _maxSamples = 1000;

        /// <summary>
        /// Obtiene la colección de eventos registrados.
        /// </summary>
        public IEnumerable<double> Events => _events;

        /// <inheritdoc/>
        public override int EventCount => _events.Count;

        /// <inheritdoc/>
        public override double AverageMs => Get(Enumerable.Average);

        /// <inheritdoc/>
        public override double MinMs => Get(Enumerable.Min);

        /// <inheritdoc/>
        public override double MaxMs => Get(Enumerable.Max);

        /// <summary>
        /// Obtiene o establece la cantidad máxima de muestras a contener en
        /// este monitor de rendimiento.
        /// </summary>
        public int MaxSamples
        {
            get => _maxSamples;
            set
            {
                lock (_events)
                {
                    _maxSamples = value;
                    while (_events.Count > value) _events.Dequeue();
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnReset()
        {
            _events.Clear();
        }

        /// <inheritdoc/>
        protected override void RegisterEvent(double milliseconds)
        {
            lock (_events)
            {
                if (_events.Count == _maxSamples) _events.Dequeue();
                _events.Enqueue(milliseconds);
            }
            Notify(nameof(Events));
        }

        private double Get(Func<IEnumerable<double>, double> func)
        {
            return _events.Any() ? func(_events) : double.NaN;
        }
    }
}