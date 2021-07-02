using System;
using System.Collections.Generic;
using Appets.Domain;

namespace Appets.BusinessLogic.Interface
{
    public interface IPostLogic : ICrud<Post>
    {
        List<Post> FilterPostsByRange(List<Post> posts, int range, string ubication);
        List<Tuple<int, Post>> GetSimilarPosts(Guid postId, Dictionary<string, int> scores, int limitSimilarPost);
        Post IgnoreSimilar(Guid idPost, Guid idIgnoredPost);
    }
}
