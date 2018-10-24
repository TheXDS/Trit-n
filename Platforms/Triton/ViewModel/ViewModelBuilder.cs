using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Core.Annotations;
using TheXDS.Triton.Core.Models.Base;

namespace TheXDS.Triton.ViewModel
{
    /// <summary>
    ///     Constructor dinámico de tipos ViewModel en Runtime.
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public static class ViewModelBuilder<TModel, TKey>
        where TModel : ModelBase<TKey>, new()
        where TKey : struct, IComparable<TKey>
    {        
        /// <summary>
        ///     Construye un nuevo ViewModel sin ninguna clase que herede
        ///     <see cref="ViewModel{TModel,TKey}"/> como base.
        /// </summary>
        /// <returns>
        ///     Un nuevo ViewModel con las propiedades originales del modelo
        ///     implementadas como llamadas con notificación de cambio de
        ///     propiedades.
        /// </returns>
        public static ViewModel<TModel, TKey> New()
        {
            return Build().New<ViewModel<TModel, TKey>>();
        }
        
        /// <summary>
        ///     Construye un nuevo ViewModel utilizando una clase base que
        ///     hereda de <see cref="ViewModel{TModel,TKey}"/> como base.
        /// </summary>
        /// <typeparam name="TViewModel">
        ///     Clase de ViewModel a utilizar como tipo base.
        /// </typeparam>
        /// <returns>
        ///     Un nuevo ViewModel con las propiedades originales del modelo
        ///     implementadas como llamadas con notificación de cambio de
        ///     propiedades.
        /// </returns>
        public static TViewModel New<TViewModel>() where TViewModel : ViewModel<TModel,TKey>
        {
            return Activator.CreateInstance(Build<TViewModel>()) as TViewModel;
        }
        
        /// <summary>
        ///     Construye un tipo de ViewModel sin ninguna clase que herede
        ///     <see cref="ViewModel{TModel,TKey}"/> como base.
        /// </summary>
        /// <returns></returns>
        public static Type Build() => Build<ViewModel<TModel, TKey>>();
        
        /// <summary>
        ///     Construye un tipo de ViewModel utilizando una clase base que
        ///     hereda de <see cref="ViewModel{TModel,TKey}"/> como base.
        /// </summary>
        /// <typeparam name="TViewModel">
        ///     Clase de ViewModel a utilizar como tipo base.
        /// </typeparam>
        /// <returns></returns>
        public static Type Build<TViewModel>() where TViewModel : ViewModel<TModel,TKey>
        {
            if (Builders.BuiltTypes.ContainsKey(typeof(TViewModel)))
                return Builders.BuiltTypes[typeof(TViewModel)];

            var tb = Builders.MBuilder.DefineType(
                $"{typeof(TModel).Name}ViewModel_{Guid.NewGuid()}",
                TypeAttributes.Public,
                typeof(TViewModel)
            );

            var ctor = tb.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                Type.EmptyTypes);
            var ctorIl = ctor.GetILGenerator();
            ctorIl.Emit(OpCodes.Ldarg_0);
            ctorIl.Emit(OpCodes.Call, typeof(TViewModel).GetConstructor(Type.EmptyTypes) ?? throw new InvalidOperationException());
            ctorIl.Emit(OpCodes.Ret);

            var addedProps = new HashSet<string>();

            foreach (var j in typeof(TModel)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && addedProps.All(q=>q!=p.Name)))
            {
                addedProps.Add(j.Name);
                AddProp(tb, j);
            }

            var retVal = tb.CreateType();
            Builders.BuiltTypes.Add(typeof(TViewModel), retVal);
            return retVal;
        }

        private static void AddProp([NotNull]TypeBuilder tb,[NotNull] PropertyInfo property) 
        {
            const MethodAttributes gsArgs = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
            var t = typeof(ViewModel<TModel, TKey>);
            var entity = t.GetProperty("Entity", BindingFlags.NonPublic|BindingFlags.Instance)?.GetMethod ?? throw new TamperException();

            var prop = tb.DefineProperty(
                property.Name,
                PropertyAttributes.HasDefault,
                property.PropertyType,
                null);
            
            var getM = tb.DefineMethod(
                $"get_{property.Name}",
                gsArgs,
                property.PropertyType,
                null
            );
            var getIl = getM.GetILGenerator();
            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Call, entity);
            getIl.Emit(OpCodes.Callvirt, property.GetMethod);
            getIl.Emit(OpCodes.Ret);
            prop.SetGetMethod(getM);
            
            var setM = tb.DefineMethod( 
                $"set_{property.Name}",
                gsArgs,
                null,
                new []{ property.PropertyType }
            );
            var setIl = setM.GetILGenerator();
            var eqM = property.PropertyType.GetMethod("Equals", new []{ property.PropertyType }) ??
                      property.PropertyType.GetMethod("Equals", new []{ typeof(object) });
            if (!(eqM is null))
            {
                var lbl1 = setIl.DefineLabel();
                setIl.Emit(OpCodes.Ldarg_0);
                setIl.Emit(OpCodes.Call, entity);
                setIl.Emit(OpCodes.Callvirt, property.GetMethod);
                setIl.Emit(OpCodes.Ldarg_1);
                setIl.Emit(OpCodes.Callvirt, eqM);
                setIl.Emit(OpCodes.Brfalse, lbl1);
                setIl.Emit(OpCodes.Ret);
                setIl.MarkLabel(lbl1);
            }
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Call, entity);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Callvirt,property.SetMethod);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldstr, property.Name);
            setIl.Emit(OpCodes.Call, t.GetMethod("OnPropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new TamperException());
            setIl.Emit(OpCodes.Ret);
            prop.SetSetMethod(setM);
        }

    }
}
