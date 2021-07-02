using Appets.BusinessLogic.Interface;
using Appets.DataAccess.Interface;
using Appets.Domain;
using Appets.Domain.Enums;
using Appets.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Appets.BusinessLogic.Test
{
    [TestClass]
    public class PostLogicTest
    {
        private Post post;
        private Post seenPost;
        private Mock<IPostRepository> mockPostRepository;
        private string location;
        private IPostLogic service;
        private Dictionary<string, int> scores;

        [TestInitialize]
        public void SetUp()
        {
            location = "37.13899, -122.019455";

            Tag tagRed = new Tag
            {
                Type = "RedColor",
                Value = "0"
            };

            Tag tagBlue = new Tag
            {
                Type = "BlueColor",
                Value = "0"
            };

            Tag tagGreen = new Tag
            {
                Type = "GreenColor",
                Value = "0"
            };

            Tag tagAnimal = new Tag
            {
                Type = "Animal",
                Value = "Perro"
            };

            Tag tagBreed = new Tag
            {
                Type = "Breed",
                Value = "Ovejero Aleman"
            };

            List<Tag> list = new List<Tag>();
            list.Add(tagRed);
            list.Add(tagBlue);
            list.Add(tagGreen);
            list.Add(tagAnimal);
            list.Add(tagBreed);

            post = new Post
            {
                Title = "Post",
                Description = "This is a post",
                Tags = list,
                ReporterPhone = "098356232",
                Ubication = location,
                Type = PostTypeEnum.LOST
            };

            seenPost = new Post
            {
                Id = new Guid("FC6FA041-1514-4C00-3A95-08D89195BF72"),
                Title = "Post",
                Description = "This is a post",
                Tags = list,
                ReporterPhone = "098356232",
                Ubication = location,
                Type = PostTypeEnum.SEEN
            };

            scores = new Dictionary<string, int>
            {
                { "Animal", 1000 },                
                { "Breed", 500 },
                { "RedColor", 255 },
                { "GreenColor", 255 },
                { "BlueColor", 255 }
            };

            mockPostRepository = new Mock<IPostRepository>(MockBehavior.Strict);
            service = new PostLogic(mockPostRepository.Object);
        }

        [TestMethod]
        public void CreatePost()
        {
            mockPostRepository.Setup(m => m.Add(this.post));
            mockPostRepository.Setup(m => m.Save());

            Post addedPost = service.Create(this.post);

            mockPostRepository.VerifyAll();
            Assert.AreEqual(addedPost.Id, this.post.Id);
        }


        [TestMethod]
        public void RemovePost()
        {
            mockPostRepository.Setup(m => m.Get(this.post.Id)).Returns(this.post);
            mockPostRepository.Setup(m => m.Remove(this.post));
            mockPostRepository.Setup(m => m.Save());

            service.Remove(this.post.Id);

            mockPostRepository.VerifyAll();
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void RemovePostNullId()
        {
            mockPostRepository.Setup(m => m.Get(this.post.Id)).Returns(this.post);
            mockPostRepository.Setup(m => m.Remove(this.post)).Throws(new ArgumentException());

            service.Remove(Guid.Empty);
        }

        [TestMethod]
        public void UpdatePost()
        {
            Post NewPost = new Post
            {
                Title = "Post updated",
                Description = "This is a post",
                Ubication = "new ubication",
                ReporterPhone = "092222222",
                Type = PostTypeEnum.LOST
            };

            mockPostRepository.Setup(m => m.Get(this.post.Id)).Returns(this.post);
            mockPostRepository.Setup(m => m.Update(this.post));
            mockPostRepository.Setup(m => m.Save());

            Post updatedPost = service.Update(post.Id, NewPost);

            mockPostRepository.VerifyAll();
            Assert.AreEqual(NewPost.Description, updatedPost.Description);
            Assert.AreEqual(NewPost.Title, updatedPost.Title);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void UpdatePostNullId()
        {
            mockPostRepository.Setup(m => m.Get(this.post.Id)).Returns(this.post);
            mockPostRepository.Setup(m => m.Update(this.post)).Throws(new ArgumentException());

            service.Update(Guid.Empty, this.post);
        }

        [ExpectedException(typeof(IncorrectParamException))]
        [TestMethod]
        public void UpdatePostInconsistentId()
        {
            Guid inconsistentGuid = Guid.NewGuid();
            mockPostRepository.Setup(m => m.Get(inconsistentGuid)).Throws(new IncorrectParamException("Id and object id doesnt match"));

            service.Update(inconsistentGuid, this.post);
        }

        [TestMethod]
        public void GetAllPosts()
        {
            List<Post> listPosts = new List<Post>();
            listPosts.Add(this.post);

            mockPostRepository.Setup(m => m.GetAll()).Returns(listPosts);

            List<Post> result = service.GetAll().ToList<Post>();

            mockPostRepository.VerifyAll();
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void GetPostById()
        {
            mockPostRepository.Setup(m => m.Get(this.post.Id)).Returns(this.post);

            Post result = service.Get(this.post.Id);

            mockPostRepository.VerifyAll();
            Assert.AreEqual(result, this.post);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void GetPostByIdNullId()
        {
            mockPostRepository.Setup(m => m.Get(this.post.Id)).Throws(new ArgumentException());
            Post result = service.Get(Guid.Empty);
        }

        [ExpectedException(typeof(IncorrectParamException))]
        [TestMethod]
        public void CoordinatesBadFormat()
        {
            service.FilterPostsByRange(new List<Post> { post }, 1, "111");
        }

        [TestMethod]
        public void FilterPostByRange()
        {
            var filteredPosts = service.FilterPostsByRange(new List<Post> { post }, 1, location);

            Assert.AreEqual(post.Id, filteredPosts.FirstOrDefault().Id);
        }

        [TestMethod]
        public void IgnoreSimilarNoIgnoredPosts()
        {
            post.Id = new Guid("9A515BC6-6B6F-4D70-1A0B-08D88E834F3F");
            mockPostRepository.Setup(m => m.Get(It.IsAny<Guid>())).Returns(post);
            mockPostRepository.Setup(m => m.Update(It.IsAny<Post>()));
            mockPostRepository.Setup(m => m.Save());

            var postWithIgnoredPost = service.IgnoreSimilar(post.Id, post.Id);

            mockPostRepository.VerifyAll();

            Assert.AreEqual(post.Id.ToString(), postWithIgnoredPost.IgnoredPosts);
        }

        [TestMethod]
        public void IgnoreSimilarIgnoredPosts()
        {
            post.Id = new Guid("9A515BC6-6B6F-4D70-1A0B-08D88E834F3F");
            mockPostRepository.Setup(m => m.Get(It.IsAny<Guid>())).Returns(post);
            mockPostRepository.Setup(m => m.Update(It.IsAny<Post>()));
            mockPostRepository.Setup(m => m.Save());

            post.IgnoredPosts = post.Id.ToString();

            var postWithIgnoredPost = service.IgnoreSimilar(post.Id, post.Id);

            mockPostRepository.VerifyAll();

            Assert.AreEqual($"{post.Id}/{post.Id}", postWithIgnoredPost.IgnoredPosts);
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectParamException))]
        public void GetSimilarPostsIsNotLostPost()
        {
            post.Id = new Guid("9A515BC6-6B6F-4D70-1A0B-08D88E834F3F");
            post.Type = PostTypeEnum.FOUND;
            mockPostRepository.Setup(m => m.Get(It.IsAny<Guid>())).Returns(post);

            service.GetSimilarPosts(post.Id, scores, 1);
        }

        [TestMethod]
        public void GetSimilarPostsNoActiveSeenPost()
        {
            post.Id = new Guid("9A515BC6-6B6F-4D70-1A0B-08D88E834F3F");
            mockPostRepository.Setup(m => m.Get(It.IsAny<Guid>())).Returns(post);
            mockPostRepository.Setup(m => m.GetAllActiveSeenPosts()).Returns(new List<Post>());

            var similarPosts = service.GetSimilarPosts(post.Id, scores, 1);

            mockPostRepository.VerifyAll();

            Assert.AreEqual(0, similarPosts.Count);
        }

        [TestMethod]
        public void GetSimilarPostsIgnoredPost()
        {
            post.Id = new Guid("9A515BC6-6B6F-4D70-1A0B-08D88E834F3F");
            post.IgnoredPosts = seenPost.Id.ToString();
            mockPostRepository.Setup(m => m.Get(It.IsAny<Guid>())).Returns(post);
            mockPostRepository.Setup(m => m.GetAllActiveSeenPosts()).Returns(new List<Post>{ seenPost });

            var similarPosts = service.GetSimilarPosts(post.Id, scores, 1);

            mockPostRepository.VerifyAll();

            Assert.AreEqual(0, similarPosts.Count);
        }

        [TestMethod]
        public void GetSimilarPosts()
        {
            post.Id = new Guid("9A515BC6-6B6F-4D70-1A0B-08D88E834F3F");
            mockPostRepository.Setup(m => m.Get(It.IsAny<Guid>())).Returns(post);
            mockPostRepository.Setup(m => m.GetAllActiveSeenPosts()).Returns(new List<Post> { seenPost });

            var similarPosts = service.GetSimilarPosts(post.Id, scores, 10);

            mockPostRepository.VerifyAll();

            Assert.AreEqual(1, similarPosts.Count);
        }
    }
}
