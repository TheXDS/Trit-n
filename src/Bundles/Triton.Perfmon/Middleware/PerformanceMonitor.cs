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
    public class PerformanceMonitor : INotifyPropertyChanged, ITransactionMiddleware
    {
        public event EventHandler<ValueEventArgs<TimeSpan>>? Elapsed;
        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly List<TimeSpan> _events = new List<TimeSpan>();
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public TimeSpan? Average => TimeSpan.FromMilliseconds(_events.Average(p => p.TotalMilliseconds));

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
                Elapsed?.Invoke(this, _stopwatch.Elapsed.PushInto(_events));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Average)));
            }
            return null;
        }
    }
}