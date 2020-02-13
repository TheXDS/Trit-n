using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using TheXDS.MCART.Exceptions;
using TheXDS.Triton.Services.Base;
using St = TheXDS.Triton.Resources.Strings;

namespace TheXDS.Triton.Exceptions
{
    /// <summary>
    /// Excepción que se produce al intentar utilizar un servicio sin
    /// configurar.
    /// </summary>
    [Serializable]
    public class UnconfiguredServiceException : OffendingException<PropertyInfo>
    {
        private static string MkMsg(string unconfiguredValue)
        {
            return string.Join(Environment.NewLine, St.UnconfiguredServiceError, string.Format(St.UnconfiguredValueX, unconfiguredValue));
        }

        private static PropertyInfo GetProp(IServiceConfiguration origin, string unconfiguredValue)
        {
            return origin.GetType().GetProperty(unconfiguredValue) ?? throw new TamperException();
        }

        public UnconfiguredServiceException(IServiceConfiguration origin, [CallerMemberName]string unconfiguredValue = null!) : base(MkMsg(unconfiguredValue), GetProp(origin, unconfiguredValue)) { }

        public UnconfiguredServiceException() : base (St.UnconfiguredServiceError) { }

        public UnconfiguredServiceException(PropertyInfo offendingObject) : base(MkMsg(offendingObject.Name)) { }

        public UnconfiguredServiceException(string message) : base(message) { }

        public UnconfiguredServiceException(string message, PropertyInfo offendingObject) : base(message, offendingObject) { }

        public UnconfiguredServiceException(Exception inner) : base(inner) { }

        public UnconfiguredServiceException(Exception inner, PropertyInfo offendingObject) : base(inner, offendingObject) { }

        public UnconfiguredServiceException(string message, Exception inner) : base(message, inner) { }

        public UnconfiguredServiceException(string message, Exception inner, PropertyInfo offendingObject) : base(message, inner, offendingObject) { }

        protected UnconfiguredServiceException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        protected UnconfiguredServiceException(SerializationInfo info, StreamingContext context, PropertyInfo offendingObject) : base(info, context, offendingObject) { }
    }
}
