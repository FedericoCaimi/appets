using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Appets.DataAccess.Interface;
using Appets.Domain;
using Appets.Exceptions;

namespace Appets.DataAccess
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(DbContext context)
        {
            this.Context = context;
        }
        public User GetByEmail(string mail)
        {
            if (!existsEmail(mail)) throw new DBIncorrectLoginException();
            return Context.Set<User>().FirstOrDefault(x => x.Email == mail);
        }
        private bool existsEmail(string mail)
        {
            return Context.Set<User>().FirstOrDefault(x => x.Email == mail) != null;
        }
        public User GetByUsername(string username)
        {
            if (!existsUsername(username)) throw new DBUsernameNotFoundException();
            return Context.Set<User>().FirstOrDefault(x => x.UserName == username);
        }
        private bool existsUsername(string username)
        {
            return Context.Set<User>().FirstOrDefault(x => x.UserName == username) != null;
        }
    }
}