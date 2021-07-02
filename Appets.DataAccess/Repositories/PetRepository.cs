using System;
using System.Collections.Generic;
using Appets.Domain;
using Microsoft.EntityFrameworkCore;

namespace Appets.DataAccess.Repositories
{
    public class PetRepository : BaseRepository<Pet>
    {
        public PetRepository(DbContext context)
        {
            this.Context = context;
        }

        public override Pet Get(Guid id)
        {
            return this.Context.Set<Pet>().Include("Posts").Include("Tags").FirstOrDefaultAsync(p => p.Id == id).Result;
        }

        public override IEnumerable<Pet> GetAll()
        {
            return this.Context.Set<Pet>().Include("Owner")
            .Include(p => p.Posts).ThenInclude(p => p.Tags)
            .Include("Tags");
        }
    }
}
