using TheXDS.MCART.Math;

namespace TheXDS.Triton.Middleware
{
    /// <summary>
    /// Middleware que permite obtener información específica sobre el
    /// tiempo que toma ejecutar acciones Crud.
    /// </summary>
    public class PerformanceMonitor : PerformanceMonitorBase
    {
        private int evt;
        private double avg;
        private double min;
        private double max;

        /// <inheritdoc/>
        public override int EventCount => evt;

        /// <inheritdoc/>
        public override double AverageMs => avg;

        /// <inheritdoc/>
        public override double MinMs => min;

        /// <inheritdoc/>
        public override double MaxMs => max;

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="PerformanceMonitor"/>.
        /// </summary>
        public PerformanceMonitor()
        {
            Reset();
        }

        /// <inheritdoc/>
        protected override void OnReset()
        {
            avg = double.NaN;
            evt = 0;
            min = double.NaN;
            max = double.NaN;
        }

        /// <inheritdoc/>
        protected override void RegisterEvent(double milliseconds)
        {
            if (!avg.IsValid()) avg = 0.0;
            if (milliseconds > max || !max.IsValid()) max = milliseconds;
            else if (milliseconds < min || !min.IsValid()) min = milliseconds;
            avg = (avg * evt + milliseconds) / (++evt);
        }
    }
}