﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TheXDS.Triton.Resources {
    using System;
    
    
    /// <summary>
    ///   Clase de recurso fuertemente tipado, para buscar cadenas traducidas, etc.
    /// </summary>
    // StronglyTypedResourceBuilder generó automáticamente esta clase
    // a través de una herramienta como ResGen o Visual Studio.
    // Para agregar o quitar un miembro, edite el archivo .ResX y, a continuación, vuelva a ejecutar ResGen
    // con la opción /str o recompile su proyecto de VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Strings {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Strings() {
        }
        
        /// <summary>
        ///   Devuelve la instancia de ResourceManager almacenada en caché utilizada por esta clase.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TheXDS.Triton.Resources.Strings", typeof(Strings).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Reemplaza la propiedad CurrentUICulture del subproceso actual para todas las
        ///   búsquedas de recursos mediante esta clase de recurso fuertemente tipado.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Se ha intentado sobreescribir información que otro usuario ya ha editado..
        /// </summary>
        internal static string ConcurrencyFailure {
            get {
                return ResourceManager.GetString("ConcurrencyFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a El servidor de datos ha devuelto un error..
        /// </summary>
        internal static string DbFailure {
            get {
                return ResourceManager.GetString("DbFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Se ha denegado el acceso a este recurso..
        /// </summary>
        internal static string FailureForbidden {
            get {
                return ResourceManager.GetString("FailureForbidden", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Se ha producido un error desconocido en la operación..
        /// </summary>
        internal static string FailureUnknown {
            get {
                return ResourceManager.GetString("FailureUnknown", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Se ha producido un error en la red..
        /// </summary>
        internal static string NetworkFailure {
            get {
                return ResourceManager.GetString("NetworkFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a La operación se ha completado satisfactoriamente..
        /// </summary>
        internal static string OperationCompletedSuccessfully {
            get {
                return ResourceManager.GetString("OperationCompletedSuccessfully", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a El servicio ha encontrado un error..
        /// </summary>
        internal static string ServiceFailure {
            get {
                return ResourceManager.GetString("ServiceFailure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Error de validación..
        /// </summary>
        internal static string ValidationError {
            get {
                return ResourceManager.GetString("ValidationError", resourceCulture);
            }
        }
    }
}
