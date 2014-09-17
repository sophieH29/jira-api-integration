using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Security;

namespace SoftServe.BL.Exceptions
{
    public class BusinessLogicException : ApplicationException
    {
        public BusinessLogicException() { }
        public BusinessLogicException(string message) : base(message) { }

        [SecuritySafeCritical]
        protected BusinessLogicException(SerializationInfo info, StreamingContext context) : base(info, context) { }
        public BusinessLogicException(string message, Exception innerException) : base(message, innerException) { }
    }
}
