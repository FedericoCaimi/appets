using System;
using System.Collections.Generic;
using Appets.Domain;

namespace Appets.BusinessLogic.Interface
{
    public interface IAuthenticationLogic
    {
        Session Login(User user);
        void Logout(Guid userId);
        bool IsLoggedIn(Guid token);
        IEnumerable<Session> GetAll();
        Session Get(Guid id);
    }
}