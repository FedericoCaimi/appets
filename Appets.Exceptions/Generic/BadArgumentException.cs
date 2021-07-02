using System;

namespace Appets.Exceptions
{
    [Serializable]
    public abstract class BadArgumentException : Exception
    {
        public BadArgumentException(string message = "")
        : base(String.Format(message)) { }
    }
}
