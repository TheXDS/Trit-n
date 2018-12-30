using System;
using System.Collections.Generic;
using System.Reflection;
using TheXDS.MCART;
using TheXDS.MCART.Attributes;
using TheXDS.Triton.Annotations;

namespace TheXDS.Triton.Component
{
    /// <summary>
    ///     Contiene un registro de los métodos con atributos de seguridad
    ///     registrados por la aplicación.
    /// </summary>
    public static class FunctionRegistry
    {
        private static readonly HashSet<Type> RegisteredTypes = new HashSet<Type>();
        private static readonly Dictionary<MethodInfo, MethodCategory> Funcs = new Dictionary<MethodInfo, MethodCategory>();

        /// <summary>
        ///     Diccionario de sólo lectura que contiene a todos los métodos
        ///     registrados para participar el el mecanismo de seguridad junto
        ///     a un valor que describe la categoría de seguridad del mismo.
        /// </summary>
        public static IReadOnlyDictionary<MethodInfo, MethodCategory> Functions => Funcs;
        
        /// <summary>
        ///     Registra los métodos del tipo especificado para participar en
        ///     el mecanismo de seguridad de <see cref="TheXDS.Triton"/>.
        /// </summary>
        /// <typeparam name="T">Tipo a registrar.</typeparam>
        [Thunk]
        public static void Register<T>()
        {
            Register(typeof(T));
        }

        /// <summary>
        ///     Registra los métodos del tipo especificado para participar en
        ///     el mecanismo de seguridad de <see cref="TheXDS.Triton"/>.
        /// </summary>
        /// <param name="t">Tipo a registrar.</param>
        public static void Register(Type t)
        {
            lock (RegisteredTypes)
            lock (Funcs)
            {
                if (RegisteredTypes.Contains(t)) return;
                RegisteredTypes.Add(t);
                foreach (var j in t.GetMethods())
                {
                    Funcs.Add(j,j.GetAttr<MethodCategoryAttribute>()?.Value ?? MethodCategory.Unspecified);
                }
            }
        }

        /// <summary>
        ///     Quita del registro del mecanismo de seguridad a los métodos del
        ///     tipo especificado.
        /// </summary>
        /// <typeparam name="T">Tipo a remover del registro.</typeparam>
        [Thunk]
        public static void UnRegister<T>()
        {
            UnRegister(typeof(T));
        }

        /// <summary>
        ///     Quita del registro del mecanismo de seguridad a los métodos del
        ///     tipo especificado.
        /// </summary>
        /// <param name="t">Tipo a remover del registro.</param>
        public static void UnRegister(Type t)
        {
            lock (RegisteredTypes)
            lock (Funcs)
            {
                if (!RegisteredTypes.Contains(t)) return;
                RegisteredTypes.Remove(t);
                foreach (var j in t.GetMethods())
                {
                    Funcs.Remove(j);
                }
            }
        }
    }
}