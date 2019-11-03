using System;
using System.Collections.Generic;
using System.Text;
using static TheXDS.MCART.Types.Extensions.EnumExtensions;
using St = TheXDS.Triton.Resources.Strings;

namespace TheXDS.Triton.Services
{

    /// <summary>
    ///     Representa el resultado devuelto por un servicio al intentar
    ///     realizar una operación.
    /// </summary>
    public class ServiceResult
    {
        /// <summary>
        ///     Obtiene un resultado simple que indica que la operación se ha
        ///     completado satisfactoriamente.
        /// </summary>
        public static ServiceResult Ok => new ServiceResult();

        /// <summary>
        ///     Enumera las posibles causas de fallo conocidas para una
        ///     operación de servicio.
        /// </summary>
        public enum FailureReason
        {
            /// <summary>
            ///     Fallo desconocido.
            /// </summary>
            Unknown,

            /// <summary>
            ///     Operación no permitida.
            /// </summary>
            Forbidden,

            /// <summary>
            ///     Error en el servicio.
            /// </summary>
            ServiceFailure,

            /// <summary>
            ///     Error en la red.
            /// </summary>
            NetworkFailure,

            /// <summary>
            ///     Error de la base de datos.
            /// </summary>
            DbFailure,

            /// <summary>
            ///     Error de validación de datos.
            /// </summary>
            ValidationError,

            /// <summary>
            ///     Error de concurrencia de datos.
            /// </summary>
            ConcurrencyFailure
        }

        /// <summary>
        ///     Obtiene un valor que indica si la operación ha sido exitosa.
        /// </summary>
        public bool Success { get; } = true;

        /// <summary>
        ///     Obtiene un mensaje que describe el resultado de la operación.
        /// </summary>
        public string Message { get; }

        /// <summary>
        ///     Obtiene la razón por la cual una operación ha fallado, o
        ///     <see langword="null"/> si la operación se completó 
        ///     exitosamente.
        /// </summary>
        public FailureReason? Reason { get; } = null;

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="ServiceResult"/>, indicando que la operación se ha
        ///     completado satisfactoriamente.
        /// </summary>
        protected ServiceResult()
        {
            Success = true;
            Message = St.OperationCompletedSuccessfully;
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="ServiceResult"/>, indicando un mensaje de estado 
        ///     personalizado a mostrar.
        /// </summary>
        /// <param name="message">Mensaje descriptivo del resultado.</param>
        public ServiceResult(string? message)
        {
            Success = true;
            Message = message ?? St.OperationCompletedSuccessfully;
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="ServiceResult"/>, especificando el motivo por el 
        ///     cual la operación ha fallado.
        /// </summary>
        /// <param name="reason">
        ///     Motivo por el cual la operación ha fallado.
        /// </param>
        public ServiceResult(in FailureReason reason)
        {
            Success = false;
            Message = (Reason = reason) switch
            {
                FailureReason.Unknown => St.FailureUnknown,
                FailureReason.Forbidden => St.FailureForbidden,
                FailureReason.ServiceFailure => St.ServiceFailure,
                FailureReason.NetworkFailure => St.NetworkFailure,
                FailureReason.DbFailure => St.DbFailure,
                FailureReason.ValidationError => St.ValidationError,
                FailureReason.ConcurrencyFailure => St.ConcurrencyFailure,
                FailureReason f => f.NameOf() ?? f.ToString("X")
            };
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="ServiceResult"/>, especificando el motivo por el 
        ///     cual la operación ha fallado, además de un mensaje descriptivo
        ///     del resultado.
        /// </summary>
        /// <param name="reason">
        ///     Motivo por el cual la operación ha fallado.
        /// </param>
        /// <param name="message">Mensaje descriptivo del resultado.</param>
        public ServiceResult(in FailureReason reason, string message)
        {
            Success = false;
            Reason = reason;
            Message = message;
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="ServiceResult"/> para una operación fallida,
        ///     extrayendo la información relevante a partir de la excepción
        ///     especificada.
        /// </summary>
        /// <param name="ex">
        ///     Excepción desde la cual obtener el mensaje y un código de error
        ///     asociado.
        /// </param>
        public ServiceResult(Exception ex)
        {
            Success = false;
            Message = ex.Message;
            Reason = (FailureReason)ex.HResult;
        }

        /// <summary>
        ///     Convierte implícitamente un <see cref="Exception"/> en un
        ///     <see cref="ServiceResult"/>.
        /// </summary>
        /// <param name="ex">
        ///     Excepción desde la cual obtener el mensaje y un código de error
        ///     asociado.
        /// </param>
        public static implicit operator ServiceResult(Exception ex) => new ServiceResult(ex);

        /// <summary>
        ///     Convierte implícitamente un <see cref="string"/> en un
        ///     <see cref="ServiceResult"/>.
        /// </summary>
        /// <param name="message">Mensaje descriptivo del resultado.</param>
        public static implicit operator ServiceResult(string message) => new ServiceResult(message);

        /// <summary>
        ///     Convierte implícitamente un <see cref="FailureReason"/> en un
        ///     <see cref="ServiceResult"/>.
        /// </summary>
        /// <param name="reason">
        ///     Motivo por el cual la operación ha fallado.
        /// </param>
        public static implicit operator ServiceResult(in FailureReason reason) => new ServiceResult(reason);

        /// <summary>
        ///     Permite utilizar un <see cref="ServiceResult"/> en una
        ///     expresión booleana.
        /// </summary>
        /// <param name="result">
        ///     Resultado desde el cual determinar el valor booleano.
        /// </param>
        public static implicit operator bool(ServiceResult result) => result.Success;

        /// <summary>
        ///     Permite utilizar un <see cref="ServiceResult"/> en una
        ///     expresión de <see cref="string"/>.
        /// </summary>
        /// <param name="result">
        ///     Resultado desde el cual extraer el mensaje.
        /// </param>
        public static implicit operator string(ServiceResult result) => result.Message;
    }

    /// <summary>
    ///     Representa el resultado de una operación de servicio que incluye un
    ///     valor devuelto.
    /// </summary>
    /// <typeparam name="T">
    ///     Tipo de resultado a devolver.
    /// </typeparam>
    public class ServiceResult<T> : ServiceResult
    {
        /// <summary>
        ///     Obtiene el valor a devolver como parte del resultado de la
        ///     operación de servicio.
        /// </summary>
        public T ReturnValue { get; } = default!;

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="ServiceResult"/>.
        /// </summary>
        public ServiceResult() : base()
        {
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="ServiceResult"/>, indicando un mensaje de estado 
        ///     personalizado a mostrar.
        /// </summary>
        /// <param name="message">Mensaje descriptivo del resultado.</param>
        public ServiceResult(string message): base(message)
        {
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="ServiceResult"/>.
        /// </summary>
        /// <param name="returnValue">
        ///     Objeto relevante retornado por la función.
        /// </param>
        public ServiceResult(T returnValue) : base()
        {
            ReturnValue = returnValue;
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="ServiceResult"/>, indicando un mensaje de estado 
        ///     personalizado a mostrar.
        /// </summary>
        /// <param name="returnValue">
        ///     Objeto relevante retornado por la función.
        /// </param>
        /// <param name="message">Mensaje descriptivo del resultado.</param>
        public ServiceResult(T returnValue, string message) : base(message)
        {
            ReturnValue = returnValue;
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="ServiceResult"/>, especificando el motivo por el 
        ///     cual la operación ha fallado.
        /// </summary>
        /// <param name="reason">
        ///     Motivo por el cual la operación ha fallado.
        /// </param>
        public ServiceResult(in FailureReason reason) : base(reason)
        {
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="ServiceResult"/>, especificando el motivo por el 
        ///     cual la operación ha fallado, además de un mensaje descriptivo
        ///     del resultado.
        /// </summary>
        /// <param name="reason">
        ///     Motivo por el cual la operación ha fallado.
        /// </param>
        /// <param name="message">Mensaje descriptivo del resultado.</param>
        public ServiceResult(in FailureReason reason, string message) : base(reason, message)
        {
        }

        /// <summary>
        ///     Inicializa una nueva instancia de la clase
        ///     <see cref="ServiceResult"/> para una operación fallida,
        ///     extrayendo la información relevante a partir de la excepción
        ///     especificada.
        /// </summary>
        /// <param name="ex">
        ///     Excepción desde la cual obtener el mensaje y un código de error
        ///     asociado.
        /// </param>
        public ServiceResult(Exception ex) : base(ex)
        {
        }

        /// <summary>
        ///     Convierte implícitamente un valor <typeparamref name="T"/> en
        ///     un <see cref="ServiceResult{T}"/>.
        /// </summary>
        /// <param name="result">
        ///     Resultado de la operación.
        /// </param>
        public static implicit operator T(ServiceResult<T> result) => result.ReturnValue;

        /// <summary>
        ///     Convierte implícitamente un <see cref="Exception"/> en un
        ///     <see cref="ServiceResult{T}"/>.
        /// </summary>
        /// <param name="ex">
        ///     Excepción desde la cual obtener el mensaje y un código de error
        ///     asociado.
        /// </param>
        public static implicit operator ServiceResult<T>(Exception ex) => new ServiceResult<T>(ex);

        /// <summary>
        ///     Convierte implícitamente un <see cref="string"/> en un
        ///     <see cref="ServiceResult{T}"/>.
        /// </summary>
        /// <param name="message">Mensaje descriptivo del resultado.</param>
        public static implicit operator ServiceResult<T>(string message) => new ServiceResult<T>(message);

        /// <summary>
        ///     Convierte implícitamente un <see cref="ServiceResult.FailureReason"/> en un
        ///     <see cref="ServiceResult{T}"/>.
        /// </summary>
        /// <param name="reason">
        ///     Motivo por el cual la operación ha fallado.
        /// </param>
        public static implicit operator ServiceResult<T>(in FailureReason reason) => new ServiceResult<T>(reason);

        /// <summary>
        ///     Permite utilizar un <see cref="ServiceResult{T}"/> en una
        ///     expresión booleana.
        /// </summary>
        /// <param name="result">
        ///     Resultado desde el cual determinar el valor booleano.
        /// </param>
        public static implicit operator bool(ServiceResult<T> result) => result.Success;

        /// <summary>
        ///     Permite utilizar un <see cref="ServiceResult{T}"/> en una
        ///     expresión de <see cref="string"/>.
        /// </summary>
        /// <param name="result">
        ///     Resultado desde el cual extraer el mensaje.
        /// </param>
        public static implicit operator string(ServiceResult<T> result) => result.Message;
    }
}