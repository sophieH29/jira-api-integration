using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Security;

namespace SoftServe.BL.Exceptions
{
    public class BusinessLogicValidationException : BusinessLogicException
    {
        public string InvalidField { get; private set;  }

        public BusinessLogicValidationException() { }
        public BusinessLogicValidationException(string message) : base(message) { }

        public BusinessLogicValidationException(string message, string fieldName) : base(message)
        {
            InvalidField = fieldName;
        }

        [SecuritySafeCritical]
        protected BusinessLogicValidationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public BusinessLogicValidationException(string message, Exception innerException) : base(message, innerException) { }

    }
}
