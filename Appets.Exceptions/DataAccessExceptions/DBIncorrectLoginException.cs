using System;

namespace Appets.Exceptions
{
    [Serializable]
    public class DBIncorrectLoginException : BadLoginException
    {
        public DBIncorrectLoginException()
        : base(String.Format("Email or password are incorrect")) { }
    }
}
