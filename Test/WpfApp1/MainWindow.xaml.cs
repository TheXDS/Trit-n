using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheXDS.Triton.Core.Models.Base;
using TheXDS.Triton.ViewModel;

namespace WpfApp1
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
    }

    public class Contact : ModelBase<int>, INameable
    {
        public static ContactViewModel MakeVm => ViewModelBuilder<Contact, int>.New<ContactViewModel>();

        /// <inheritdoc />
        /// <summary>
        ///     Obtiene o establece el nombre de la entidad.
        /// </summary>
        public string Name { get; set; } = "Nombre";

        public string Phone { get; set; } = "Teléfono";

    }

    public class ContactViewModel : ViewModel<Contact, int>
    {
        public bool HasPhone => string.IsNullOrWhiteSpace(Entity.Phone);
    }
}
