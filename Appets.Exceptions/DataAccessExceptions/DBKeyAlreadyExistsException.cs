using System;

namespace Appets.Exceptions
{
    [Serializable]
    public class DBKeyAlreadyExistsException : AlreadyExistsException
    {
        public DBKeyAlreadyExistsException()
        : base(String.Format("Id already exists in the system")) { }
    }
}
