using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using TheXDS.Triton.Core.Models.Base;

namespace TheXDS.Triton.ViewModel
{
    public static class ViewModelBuilder<TModel, TKey>
        where TModel : ModelBase<TKey>, new()
        where TKey : struct, IComparable<TKey>
    {
//        public static Type Build<TModel, TKey>()
//        {
//            var aName = new AssemblyName("TheXDS.Triton.ViewModel");
//            var ab = AppDomain.CurrentDomain.DefineDynamicAssembly(
//                aName, AssemblyBuilderAccess.Run);
//
//            var mb = ab.DefineDynamicModule(aName.Name, $"{aName.Name}.dll");
//
//            var tb = mb.DefineType(
//                $"{typeof(TModel).Name}ViewModel",
//                TypeAttributes.Public
//            );
//
//            foreach (var j in typeof(TModel)
//                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
//                .Where(p => p.CanRead && p.CanWrite))
//            {
//                
//            }
//        }



        public static void AddProp(TypeBuilder tb, PropertyInfo property) 
        {
            const MethodAttributes gsArgs = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;
            
            var field = tb.DefineField(
                $"_{property.Name}",
                property.PropertyType,
                FieldAttributes.Private
            );
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
            var getIL = getM.GetILGenerator();
            getIL.Emit(OpCodes.Ldarg_0);
            getIL.Emit(OpCodes.Call, typeof(ViewModelBase<TModel,TKey>).GetProperty("Entity").GetMethod);
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
            var lbl1 = setIL.DefineLabel();
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Call, typeof(ViewModelBase<TModel,TKey>).GetProperty("Entity").GetMethod);
            setIL.Emit(OpCodes.Callvirt, property.GetMethod);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Callvirt, typeof(string).GetMethod("Equals"));
            setIL.Emit(OpCodes.Brfalse, lbl1);
            setIL.Emit(OpCodes.Ret);
            setIL.MarkLabel(lbl1);
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Call, typeof(ViewModelBase<TModel,TKey>).GetProperty("Entity").GetMethod);
            setIL.Emit(OpCodes.Ldarg_1);
            setIL.Emit(OpCodes.Callvirt,property.SetMethod);
            setIL.Emit(OpCodes.Ldarg_0);
            setIL.Emit(OpCodes.Ldstr, property.Name);
            setIL.Emit(OpCodes.Call, typeof(ViewModelBase<TModel,TKey>).GetMethod());

        }
    }
}
