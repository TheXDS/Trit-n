using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Windows.Input;
using TheXDS.MCART.Types;
using TheXDS.MCART.Types.Extensions;
using TheXDS.MCART.ViewModel;
using TheXDS.Triton.Component;
using static TheXDS.MCART.Types.Extensions.ObservingCommandExtensions;
using St = TheXDS.Triton.Resources.UiStrings;

namespace TheXDS.Triton.ViewModels
{
    public class PageContainerViewModel<THost> : ViewModelBase, IEnumerable<PageViewModel<THost>>, INotifyCollectionChanged where THost : IVisualHost, new()
    {
        private readonly ObservableCollection<PageViewModel<THost>> _children = new ObservableCollection<PageViewModel<THost>>();

        public void AddPage(PageViewModel<THost> page)
        {
            _children.Add(page);
        }

        public void RemovePage(PageViewModel<THost> page)
        {
            _children.Remove(page);
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { _children.CollectionChanged += value; }
            remove { _children.CollectionChanged -= value; }
        }

        IEnumerator<PageViewModel<THost>> IEnumerable<PageViewModel<THost>>.GetEnumerator() => _children.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _children.GetEnumerator();
    }

    /// <summary>
    ///     ViewModel que describe una página visual.
    /// </summary>
    public class PageViewModel : ViewModelBase
    {
        private string? _title;
        private bool _closeable = true;
        private Color? _accentColor;
        private readonly ICloseable _host;

        /// <summary>
        ///     Obtiene o establece el valor Title.
        /// </summary>
        /// <value>El valor de Title.</value>
        public string Title
        {
            get => _title ?? St.UntitledPage;
            protected set => Change(ref _title, value);
        }

        /// <summary>
        ///     Obtiene o establece el valor Closeable.
        /// </summary>
        /// <value>El valor de Closeable.</value>
        public bool Closeable
        {
            get => _closeable;
            protected set => Change(ref _closeable, value);
        }

        /// <summary>
        ///     Obtiene o establece un color decorativo a utilizar para la página.
        /// </summary>
        /// <value>El color decorativo a utilizar.</value>
        public Color? AccentColor
        {
            get => _accentColor;
            protected set => Change(ref _accentColor, value);
        }

        /// <summary>
        ///     Obtiene el comando a ejecutar para cerrar esta página.
        /// </summary>
        public ICommand CloseCommand { get; }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="PageViewModel{T}"/>.
        /// </summary>
        public PageViewModel()
        {
            _host = UiBuilder.Builder.BuildHost(this);
            CloseCommand = new ObservingCommand(this, Close).ListensToCanExecute(() => Closeable);
        }

        protected virtual void OnClosing(ref bool cancel) { }
        protected virtual void OnClosed() { }

        protected void Close()
        {
            var cancel = false;
            OnClosing(ref cancel);
            if (cancel) return;
            _host.Close();
            OnClosed();
        }
    }
}