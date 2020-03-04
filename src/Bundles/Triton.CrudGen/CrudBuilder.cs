using System;
using System.Collections.Generic;
using System.Text;
using TheXDS.Triton.Ui.ViewModels;
using TheXDS.Triton.CrudGen.Base;
using TheXDS.Triton.Models.Base;
using TheXDS.MCART.ViewModel;
using System.Linq.Expressions;

namespace TheXDS.Triton.CrudGen
{
    public interface ICrudBuilder
    {
        PageViewModel Build();
    }

    public interface ICrudDescriptionBuilder<TModel> where TModel : Model
    {
        IPropertyDescriptor<TModel, TProperty, ViewModel<TModel>> Property<TProperty>(Expression<Func<TModel, TProperty>> selector);
    }

    public abstract class CrudBuilder<TModel> : ICrudBuilder where TModel : Model
    {
        public PageViewModel Build()
        {
        }

        protected abstract void Describe(ICrudDescriptionBuilder<TModel> editor, ICrudDescriptionBuilder<TModel> viewer);
    }
}
