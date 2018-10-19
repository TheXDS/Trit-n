using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Types.Extensions;
using TheXDS.Triton.Core.Annotations;
using TheXDS.Triton.Core.Models.Base;
using PropertyAttributes = System.Reflection.PropertyAttributes;

namespace TheXDS.Triton.ViewModel
{
    public static class ViewModelBuilder<TModel, TKey>
        where TModel : ModelBase<TKey>, new()
        where TKey : struct, IComparable<TKey>
    {
        public static ViewModelBase<TModel, TKey> New()
        {
            return Build().New<ViewModelBase<TModel, TKey>>();
        }
        public static Type Build()
        {
            const string Namespace = "TheXDS.Triton.ViewModel";

            var aName = new AssemblyName(Namespace);
            var ab = AppDomain.CurrentDomain.DefineDynamicAssembly(
                aName, AssemblyBuilderAccess.Run);

            var mb = ab.DefineDynamicModule(aName.Name);

            var tb = mb.DefineType(
                $"{typeof(TModel).Name}ViewModel",
                TypeAttributes.Public,
                typeof(ViewModelBase<TModel,TKey>)
            );

            var ctor = tb.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                Type.EmptyTypes);
            var ctorIl = ctor.GetILGenerator();
            ctorIl.Emit(OpCodes.Ldarg_0);
            ctorIl.Emit(OpCodes.Call, typeof(ViewModelBase<TModel, TKey>).GetConstructor(Type.EmptyTypes) ?? throw new InvalidOperationException());
            ctorIl.Emit(OpCodes.Ret);


            foreach (var j in typeof(TModel)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite))
            {
                AddProp(tb, j);
            }

            return tb.CreateType();
        }

        public static void AddProp([NotNull]TypeBuilder tb,[NotNull] PropertyInfo property) 
        {
            const MethodAttributes gsArgs = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
            var t = typeof(ViewModelBase<TModel, TKey>);
            var entity = t.GetProperty("Entity", BindingFlags.NonPublic|BindingFlags.Instance)?.GetMethod ?? throw new TamperException();

            var prop = tb.DefineProperty(
                property.Name,
                PropertyAttributes.HasDefault,
                //CallingConventions.HasThis,
                property.PropertyType,
                null);
            
            var getM = tb.DefineMethod(
                $"get_{property.Name}",
                gsArgs,
                property.PropertyType,
                null
            );
            var getIL = getM.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Call, entity);
            getIL.Emit(OpCodes.Callvirt, property.GetMethod);
            getIL.Emit(OpCodes.Ret);
            prop.SetGetMethod(getM);
            
            var setM = tb.DefineMethod( 
                $"set_{property.Name}",
                gsArgs,
                null,
                new []{ property.PropertyType }
            );
            var setIL = setM.GetILGenerator();
            var eqM = property.PropertyType.GetMethod("Equals", new []{ property.PropertyType }) ??
                      property.PropertyType.GetMethod("Equals", new []{ typeof(object) });
            if (!(eqM is null))
            {
                var lbl1 = setIL.DefineLabel();
                setIL.Emit(OpCodes.Ldarg_0);
                setIL.Emit(OpCodes.Call, entity);
                setIL.Emit(OpCodes.Callvirt, property.GetMethod);
                setIL.Emit(OpCodes.Ldarg_1);
                setIL.Emit(OpCodes.Callvirt, eqM);
                setIL.Emit(OpCodes.Brfalse, lbl1);
                setIL.Emit(OpCodes.Ret);
                setIL.MarkLabel(lbl1);
            }
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Call, entity);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Callvirt,property.SetMethod);
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldstr, property.Name);
            setIL.Emit(OpCodes.Call, t.GetMethod("OnPropertyChanged", BindingFlags.NonPublic | BindingFlags.Instance) ?? throw new TamperException());
            setIL.Emit(OpCodes.Ret);
            prop.SetSetMethod(setM);
        }
    }
}
