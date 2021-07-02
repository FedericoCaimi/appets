using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Appets.Domain;
using Appets.Exceptions;

namespace Appets.DataAccess
{
    public class SessionRepository : BaseRepository<Session>
    {
        public SessionRepository(DbContext context)
        {
            this.Context = context;
        }
        public override Session Get(Guid id)
        {
            if (!exists(id)) throw new DBKeyNotFoundException();
            return Context.Set<Session>().FirstOrDefault(x => x.Id == id);
        }
    }
}