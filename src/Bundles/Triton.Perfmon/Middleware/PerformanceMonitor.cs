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
    /*
     * Esta clase ejemplifica una implementación simple de Middlewares por medio de la interfaz ITransactionMiddleware.
     */

    /// <summary>
    ///     Middleware que permite obtener información específica sobre el
    ///     tiempo que toma ejecutar acciones Crud.
    /// </summary>
    public class PerformanceMonitor : INotifyPropertyChanged, ITransactionMiddleware
    {
        public event EventHandler<ValueEventArgs<double>>? Elapsed;
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly List<double> _events = new List<double>();
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public int EventCount => _events.Count;
        public double AverageMs => Get(Enumerable.Average);
        public double MinMs => Get(Enumerable.Min);
        public double MaxMs => Get(Enumerable.Max);

        private double Get(Func<List<double>,double> func)
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

    public static class NOPSimulator
    {

    }
}