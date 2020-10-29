using System;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Types.Extensions;
using St = TheXDS.Triton.Resources.Strings;

namespace TheXDS.Triton.Exceptions
{
    /// <summary>
    /// Excepción que se produce al intentar referencia o acceder a un servicio
    /// que no ha sido cargado.
    /// </summary>
    public class MissingServiceException : MissingTypeException
    {
        private static string MkMsg(Type missingSvc)
        {
            return string.Format(St.MissingXService, missingSvc.NameOf());
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="MissingServiceException"/>.
        /// </summary>
        public MissingServiceException() : base(St.MissingService)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="MissingServiceException"/>.
        /// </summary>
        /// <param name="type">Tipo que describe al servicio al cual se ha intentado hace rreferencia.</param>
        public MissingServiceException(Type type) : base(MkMsg(type), type)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="MissingServiceException"/>.
        /// </summary>
        /// <param name="message">Mensaje que describe a esta excepción.</param>
        public MissingServiceException(string message) : base(message)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="MissingServiceException"/>.
        /// </summary>
        /// <param name="inner">Excepción que es la causa de esta excepción.</param>
        public MissingServiceException(Exception inner) : base(St.MissingService, inner)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="MissingServiceException"/>.
        /// </summary>
        /// <param name="message">Mensaje que describe a esta excepción.</param>
        /// <param name="type">Tipo que describe al servicio al cual se ha intentado hace rreferencia.</param>
        public MissingServiceException(string message, Type type) : base(message, type)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="MissingServiceException"/>.
        /// </summary>
        /// <param name="inner">Excepción que es la causa de esta excepción.</param>
        /// <param name="type">Tipo que describe al servicio al cual se ha intentado hace rreferencia.</param>
        public MissingServiceException(Exception inner, Type type) : base(MkMsg(type), inner, type)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="MissingServiceException"/>.
        /// </summary>
        /// <param name="message">Mensaje que describe a esta excepción.</param>
        /// <param name="inner">Excepción que es la causa de esta excepción.</param>
        public MissingServiceException(string message, Exception inner) : base(message, inner)
        {
        }

        /// <summary>
        /// Inicializa una nueva instancia de la clase 
        /// <see cref="MissingServiceException"/>.
        /// </summary>
        /// <param name="message">Mensaje que describe a esta excepción.</param>
        /// <param name="inner">Excepción que es la causa de esta excepción.</param>
        /// <param name="type">Tipo que describe al servicio al cual se ha intentado hace rreferencia.</param>
        public MissingServiceException(string message, Exception inner, Type type) : base(message, inner, type)
        {
        }
    }
}
