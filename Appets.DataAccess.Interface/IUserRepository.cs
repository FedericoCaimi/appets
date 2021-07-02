using System;
using System.Collections.Generic;
using Appets.Domain;

namespace Appets.DataAccess.Interface
{
    public interface IUserRepository : IRepository<User>
    {
        User GetByEmail(string mail);
        User GetByUsername(string username);
    }
}