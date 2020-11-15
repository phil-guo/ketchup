using System;

namespace Ketchup.Core.Exceptions
{
    public class KetchupPlatformException : Exception
    {
        public KetchupPlatformException(string message, Exception innerException = null)
            : base(message, innerException)
        {
        }
    }
}
