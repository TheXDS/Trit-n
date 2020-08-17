using System;
using TheXDS.MCART.Exceptions;
using TheXDS.MCART.Types.Extensions;
using St = TheXDS.Triton.Resources.Strings;

namespace TheXDS.Triton.Exceptions
{
    public class MissingServiceException : MissingTypeException
    {
        private static string MkMsg(Type missingSvc)
        {
            return string.Format(St.MissingXService, missingSvc.NameOf());
        }

        public MissingServiceException() : base(St.MissingService)
        {
        }

        public MissingServiceException(Type type) : base(MkMsg(type), type)
        {
        }

        public MissingServiceException(string message) : base(message)
        {
        }

        public MissingServiceException(Exception inner) : base(St.MissingService, inner)
        {
        }

        public MissingServiceException(string message, Type type) : base(message, type)
        {
        }

        public MissingServiceException(Exception inner, Type type) : base(MkMsg(type), inner, type)
        {
        }

        public MissingServiceException(string message, Exception inner) : base(message, inner)
        {
        }

        public MissingServiceException(string message, Exception inner, Type type) : base(message, inner, type)
        {
        }
    }
}
