
using System;
using System.Collections.Generic;
using Appets.Domain;
using Appets.Domain.Enums;
using Appets.BusinessLogic.Interface;
using Appets.DataAccess.Interface;
using System.Linq;
using Appets.Exceptions;
using System.Device.Location;
using System.Globalization;

namespace Appets.BusinessLogic
{
    public class PostLogic : IPostLogic
    {
        private IPostRepository Repository;
        public PostLogic(IPostRepository repository)
        {
            this.Repository = repository;
        }

        public Post Create(Post post)
        {
            Post newPost = new Post()
            {
                Title = post.Title,
                Image = post.Image,
                Description = post.Description,
                Tags = post.Tags,
                ReporterPhone = post.ReporterPhone,
                Ubication = post.Ubication,
                Type = post.Type
            };
            Repository.Add(newPost);
            Repository.Save();
            return newPost;
        }

        public Post Get(Guid id)
        {
            return Repository.Get(id);
        }

        public IEnumerable<Post> GetAll() => Repository.GetAll();

        public void Remove(Guid id)
        {
            Post postFinded = Repository.Get(id);
            Repository.Remove(postFinded);
            Repository.Save();
        }

        public Post Update(Guid id, Post post)
        {
            if (id != post.Id) throw new IncorrectParamException("Id and object id doesnt match");

            Post postToUpdate = this.Repository.Get(id);

            postToUpdate.Title = post.Title;
            postToUpdate.Image = post.Image;
            postToUpdate.Description = post.Description;
            postToUpdate.Ubication = post.Ubication;
            postToUpdate.ReporterPhone = post.ReporterPhone;
            postToUpdate.Type = post.Type;
            postToUpdate.Tags = post.Tags;

            Repository.Update(postToUpdate);
            Repository.Save();
            return post;
        }

        public List<Post> FilterPostsByRange(List<Post> posts, int range, string ubication)
        {
            string[] coordinates;
            double lat;
            double lon;
            int rangeInMeters = range * 10000;
            try
            {
                coordinates = ubication.Split(',');
                lat = Double.Parse(coordinates[0], CultureInfo.InvariantCulture);
                lon = Double.Parse(coordinates[1], CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                throw new IncorrectParamException("Bad ubication format, must be '<lat>,<lon>'");
            }
            List<Post> filteredPosts = new List<Post>();
            GeoCoordinate userUbication = new GeoCoordinate(lat, lon);
            foreach (Post currentPost in posts)
            {
                string[] currentCoordinates = currentPost.Ubication.Split(',');
                double currentLat = Double.Parse(currentCoordinates[0], CultureInfo.InvariantCulture);
                double currentLon = Double.Parse(currentCoordinates[1], CultureInfo.InvariantCulture);
                GeoCoordinate currentPostUbication = new GeoCoordinate(currentLat, currentLon);
                double distance = userUbication.GetDistanceTo(currentPostUbication);
                if (distance <= rangeInMeters) filteredPosts.Add(currentPost);
            }
            return filteredPosts;
        }

        public List<Tuple<int, Post>> GetSimilarPosts(Guid postId, Dictionary<string, int> scores, int limitSimilarPost)
        {
            Post myPost = this.Get(postId);
            if (myPost.Type != PostTypeEnum.LOST) throw new IncorrectParamException("Service only for LOST posts");

            int maxMatch = scores.Sum(tag => tag.Value);
            List<Post> activeSeenPosts = this.Repository.GetAllActiveSeenPosts().ToList();

            removeIgnoredPosts(myPost, activeSeenPosts);

            List<Tuple<int, Post>> similarPosts = new List<Tuple<int, Post>>();

            if (activeSeenPosts.Count > 0)
            {
                fillSimilarPostList(scores, limitSimilarPost, myPost, maxMatch, activeSeenPosts, similarPosts);
            }

            similarPosts = similarPosts.OrderByDescending(post => post.Item1).ToList();
            return similarPosts;
        }

        private void fillSimilarPostList(Dictionary<string, int> scores, int limitSimilarPost, Post myPost, int maxMatch, List<Post> activeSeenPosts, List<Tuple<int, Post>> similarPosts)
        {
            foreach (Post currentPost in activeSeenPosts)
            {
                int currentScore = 0;
                foreach (KeyValuePair<string, int> tagScore in scores)
                {
                    Tag currentScoreTag = new Tag()
                    {
                        Type = tagScore.Key,
                    };

                    Tag postTag = getTag(currentPost.Tags, currentScoreTag);
                    Tag myTag = getTag(myPost.Tags, currentScoreTag);

                    if (postTag != null && myTag != null)
                        if (isColorTag(postTag) && isColorTag(myTag))
                        {
                            int difference = Math.Abs(Int32.Parse(postTag.Value) - Int32.Parse(myTag.Value));
                            int score = tagScore.Value - difference;
                            currentScore += score;
                        }
                        else if (myTag.Value == postTag.Value)
                        {
                            currentScore += tagScore.Value;
                        }
                }

                if (currentScore >= limitSimilarPost)
                {
                    int coincidencePercentage = (100 * currentScore) / maxMatch;
                    Tuple<int, Post> currentResult = new Tuple<int, Post>(coincidencePercentage, currentPost);
                    similarPosts.Add(currentResult);
                }
            }
        }

        private static void removeIgnoredPosts(Post myPost, List<Post> activeSeenPosts)
        {
            if (myPost.IgnoredPosts != null && myPost.IgnoredPosts != "")
            {
                string[] ignored = myPost.IgnoredPosts.Split('/');
                if (ignored != null && ignored.Length > 0)
                {
                    activeSeenPosts.RemoveAll(p => myPost.IgnoredPosts.Contains(p.Id.ToString()));
                }
            }
        }

        private bool isColorTag(Tag tag)
        {
            bool isColor = tag.Type == "RedColor" || tag.Type == "GreenColor" || tag.Type == "BlueColor";
            return isColor;
        }

        private Tag getTag(List<Tag> tagList, Tag tagWithType)
        {
            Tag postTag = null;

            postTag = tagList.Find(tag => tag.Type == tagWithType.Type);

            return postTag;
        }

        public Post IgnoreSimilar(Guid idPost, Guid idIgnoredPost)
        {
            Post post = Repository.Get(idPost);

            if (post.IgnoredPosts == null || post.IgnoredPosts == "")
            {
                post.IgnoredPosts = idIgnoredPost.ToString();
            }
            else
            {
                post.IgnoredPosts += "/" + idIgnoredPost;
            }
            Repository.Update(post);
            Repository.Save();
            return post;
        }

    }
}
