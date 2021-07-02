using System.Collections.Generic;
using Appets.Domain;

namespace Appets.DataAccess.Interface
{
    public interface IPostRepository : IRepository<Post>
    {
        IEnumerable<Post> GetAllActiveSeenPosts();
    }
}
