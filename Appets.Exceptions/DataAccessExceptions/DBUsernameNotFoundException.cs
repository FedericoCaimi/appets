using System;

namespace Appets.Exceptions
{
    [Serializable]
    public class DBUsernameNotFoundException : NotFoundException
    {
        public DBUsernameNotFoundException()
        : base(String.Format("Username not found")) { }
    }
}
