using Appets.Domain;
using System;

namespace Appets.WebApi.Models
{
    public class AuthenticationOut
    {
        public Guid Token { get; set; }
        public Guid UserId { get; set; }
        public DateTime DateLogged { get; set; }
        public bool IsDeleted { get; set; }

        public AuthenticationOut(Session session)
        {
            Token = session.Id;
            UserId = session.UserId;
            DateLogged = session.DateLogged;
            IsDeleted = session.IsDeleted;
        }

    }
}