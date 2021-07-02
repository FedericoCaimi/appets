using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Appets.Domain;
using Appets.Domain.Enums;
using Appets.Exceptions;
using Appets.DataAccess.Interface;

namespace Appets.DataAccess
{
    public class PostRepository : BaseRepository<Post>, IPostRepository
    {
        public PostRepository(DbContext context)
        {
            this.Context = context;
        }

        public IEnumerable<Post> GetAllActiveSeenPosts()
        {
            return Context.Set<Post>().Where(post => post.Type == PostTypeEnum.SEEN && !post.IsDeleted).Include(x => x.Tags);
        }

        public override Post Get(Guid id)
        {
            if (!exists(id)) throw new DBKeyNotFoundException();
            return Context.Set<Post>()
            .Include(x => x.Tags)
            .FirstOrDefault(x => x.Id == id);
        }

        public override IEnumerable<Post> GetAll()
        {
            return Context.Set<Post>()
            .Include(x => x.Tags);
        }
    }
}
