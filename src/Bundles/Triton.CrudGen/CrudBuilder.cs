using System;
using System.Collections.Generic;
using System.Text;
using TheXDS.Triton.Ui.ViewModels;
using TheXDS.Triton.CrudGen.Base;
using TheXDS.Triton.Models.Base;
using TheXDS.MCART.ViewModel;
using System.Linq.Expressions;
using TheXDS.MCART.Types.Extensions;

namespace TheXDS.Triton.CrudGen
{
    public interface ICrudBuilder
    {
        PageViewModel Build();
    }

    /// <summary>
    /// Define una serie de miembros a implementar por un tipo que permita 
    /// establecer un contexto de datos para utilizar internamente.
    /// </summary>
    /// <remarks>
    /// Para C#9, esta interfaz es candidato a mudarse a la característica de
    /// "shapes".
    /// </remarks>
    public interface IDataContext
    {
        /// <summary>
        /// Obtiene o establece el contexto de datos utilizado por esta
        /// instancia.
        /// </summary>
        object? DataContext { get; set; }
    }

    public interface ICrudDescriptionBuilder<TModel> where TModel : Model
    {
        IPropertyDescriptor<TModel, TProperty, ViewModel<TModel>> Property<TProperty>(Expression<Func<TModel, TProperty>> selector);
        void CustomBlock<TUi>(Expression<Func<TModel, object>> dataContext) where TUi : IDataContext, new();

    }

    public abstract class CrudBuilder<TModel> : ICrudBuilder where TModel : Model
    {
        public PageViewModel Build()
        {
        }


        protected abstract void Describe(ICrudDescriptionBuilder<TModel> editor, ICrudDescriptionBuilder<TModel> viewer);


        public string FriendlyName { get; }
        public Func<TModel, bool>? CanCreate { get; }
        public Func<TModel, bool>? CanEdit { get; }
        public Func<TModel, bool>? CanDelete { get; }

        public CrudBuilder()
        {
            FriendlyName = typeof(TModel).NameOf();            
        }

        public CrudBuilder(string friendlyName)
        {
            FriendlyName = friendlyName ?? throw new ArgumentNullException(nameof(friendlyName));
        }
    }
}
