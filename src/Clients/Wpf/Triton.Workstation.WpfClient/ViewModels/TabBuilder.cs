using System.Windows.Controls;
using TheXDS.Triton.Pages;
using TheXDS.Triton.Ui.Component;
using TheXDS.Triton.Ui.ViewModels;

namespace TheXDS.Triton.WpfClient.ViewModels
{
    /// <summary>
    /// Constructor visual que genera páginas en pestañas utilizando controles
    /// <see cref="TabHost"/> como el host primario y <see cref="Page"/> como
    /// contenido visual.
    /// </summary>
    public class TabBuilder : IVisualBuilder<TabHost>
    {
        private readonly IVisualResolver<Page> _resolver;

        /// <summary>
        /// Inicializa una nueva instancia de la clase
        /// <see cref="TabBuilder"/>, especificando el
        /// <see cref="IVisualResolver{T}"/> fuertemente tipeado a utilizar
        /// para obtener páginas a partir de un <see cref="PageViewModel"/>.
        /// </summary>
        /// <param name="resolver">
        /// <see cref="IVisualResolver{T}"/> fuertemente tipeado a utilizar
        /// para obtener páginas a partir de un <see cref="PageViewModel"/>.
        /// </param>
        public TabBuilder(IVisualResolver<Page> resolver)
        {
            _resolver = resolver;
        }
        
        /// <inheritdoc/>
        public TabHost Build(PageViewModel viewModel)
        {
            return new TabHost(viewModel, _resolver.ResolveVisual(viewModel));
        }
    }
}