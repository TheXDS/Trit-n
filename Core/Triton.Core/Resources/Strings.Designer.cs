﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TheXDS.Triton.Core.Resources {
    using System;
    
    
    /// <summary>
    ///   Clase de recurso fuertemente tipado, para buscar cadenas traducidas, etc.
    /// </summary>
    // StronglyTypedResourceBuilder generó automáticamente esta clase
    // a través de una herramienta como ResGen o Visual Studio.
    // Para agregar o quitar un miembro, edite el archivo .ResX y, a continuación, vuelva a ejecutar ResGen
    // con la opción /str o recompile su proyecto de VS.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
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
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TheXDS.Triton.Core.Resources.Strings", typeof(Strings).Assembly);
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
        ///   Busca una cadena traducida similar a Ha ocurrido un error notificando a la aplicación sobre el cambio en la propiedad &apos;{0}&apos;..
        /// </summary>
        internal static string Err_Invoking_OnPropertyChanged {
            get {
                return ResourceManager.GetString("Err_Invoking_OnPropertyChanged", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Este servicio no maneja entidades del tipo solicitado..
        /// </summary>
        internal static string Service_doesnt_handle_model {
            get {
                return ResourceManager.GetString("Service_doesnt_handle_model", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a Servicio de {0}.
        /// </summary>
        internal static string Service_Friendly_Name {
            get {
                return ResourceManager.GetString("Service_Friendly_Name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a La operación ha fallado debido a un error general de la aplicación..
        /// </summary>
        internal static string Service_Result_AppFault {
            get {
                return ResourceManager.GetString("Service_Result_AppFault", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a No cuenta con los permisos suficientes para realizar esta operación..
        /// </summary>
        internal static string Service_Result_Forbidden {
            get {
                return ResourceManager.GetString("Service_Result_Forbidden", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a La operación no se llevó a cabo debido a que no afectaba a ningún objeto..
        /// </summary>
        internal static string Service_Result_NoMatch {
            get {
                return ResourceManager.GetString("Service_Result_NoMatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a La operación se ha completado satisfactoriamente..
        /// </summary>
        internal static string Service_Result_Ok {
            get {
                return ResourceManager.GetString("Service_Result_Ok", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a El servidor ha encontrado un error procesando la solicitud..
        /// </summary>
        internal static string Service_Result_ServerFault {
            get {
                return ResourceManager.GetString("Service_Result_ServerFault", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a La operación ha fallado con un error desconocido: Result &apos;{0}&apos;.
        /// </summary>
        internal static string Service_Result_Unk {
            get {
                return ResourceManager.GetString("Service_Result_Unk", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a No ha sido posible contactar con el servidor..
        /// </summary>
        internal static string Service_Result_Unreachable {
            get {
                return ResourceManager.GetString("Service_Result_Unreachable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Busca una cadena traducida similar a La operación ha fallado debido a un error de validación de datos..
        /// </summary>
        internal static string Service_Result_ValidationFault {
            get {
                return ResourceManager.GetString("Service_Result_ValidationFault", resourceCulture);
            }
        }
    }
}
