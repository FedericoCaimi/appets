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
    public class UserLogicTest
    {
        private User user;
        private Mock<IUserRepository> mockUserRepository;
        private IUserLogic service;

        [TestInitialize]
        public void SetUp()
        {
            Tag tag = new Tag
            {
                Type = "color",
                Value = "black"
            };

            user = new User
            {
                Email = "juan@gmail.com",
                UserName = "jperez",
                Password = "perez123",
                Phone = "099999999",
                Pets = new List<Pet>(),
                Post = new List<Post>()
            };

            mockUserRepository = new Mock<IUserRepository>(MockBehavior.Strict);
            service = new UserLogic(mockUserRepository.Object);
        }

        [TestMethod]
        public void CreateUser()
        {
            mockUserRepository.Setup(m => m.Add(this.user));
            mockUserRepository.Setup(m => m.Save());

            User addedUser = service.Create(this.user);

            mockUserRepository.VerifyAll();
            Assert.AreEqual(addedUser.Id, this.user.Id);
        }


        [TestMethod]
        public void RemoveUser()
        {
            mockUserRepository.Setup(m => m.Get(this.user.Id)).Returns(this.user);
            mockUserRepository.Setup(m => m.Remove(this.user));
            mockUserRepository.Setup(m => m.Save());

            service.Remove(this.user.Id);

            mockUserRepository.VerifyAll();
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void RemoveUserNullId()
        {
            mockUserRepository.Setup(m => m.Get(this.user.Id)).Returns(this.user);
            mockUserRepository.Setup(m => m.Remove(this.user)).Throws(new ArgumentException());

            service.Remove(Guid.Empty);
        }

        [TestMethod]
        public void UpdateUser()
        {
            User NewUser = new User
            {
                Email = "juan@updated.com",
                UserName = "updated",
                Password = "updated123",
                Phone = "088888888",
                Pets = new List<Pet>(),
                Post = new List<Post>()
            };

            mockUserRepository.Setup(m => m.Get(this.user.Id)).Returns(this.user);
            mockUserRepository.Setup(m => m.Update(this.user));
            mockUserRepository.Setup(m => m.Save());

            User updatedUser = service.Update(user.Id, NewUser);

            mockUserRepository.VerifyAll();
            Assert.AreEqual(NewUser.Email, updatedUser.Email);
            Assert.AreEqual(NewUser.Password, updatedUser.Password);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void UpdateUserNullId()
        {
            mockUserRepository.Setup(m => m.Get(this.user.Id)).Returns(this.user);
            mockUserRepository.Setup(m => m.Update(this.user)).Throws(new ArgumentException());

            service.Update(Guid.Empty, this.user);
        }

        [ExpectedException(typeof(IncorrectParamException))]
        [TestMethod]
        public void UpdateUserInconsistentId()
        {
            Guid inconsistentGuid = Guid.NewGuid();
            mockUserRepository.Setup(m => m.Get(inconsistentGuid)).Throws(new IncorrectParamException("Id and object id doesnt match"));

            service.Update(inconsistentGuid, this.user);
        }

        [TestMethod]
        public void GetAllUsers()
        {
            List<User> listUsers = new List<User>();
            listUsers.Add(this.user);

            mockUserRepository.Setup(m => m.GetAll()).Returns(listUsers);

            List<User> result = service.GetAll().ToList<User>();

            mockUserRepository.VerifyAll();
            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void GetPostById()
        {
            mockUserRepository.Setup(m => m.Get(this.user.Id)).Returns(this.user);

            User result = service.Get(this.user.Id);

            mockUserRepository.VerifyAll();
            Assert.AreEqual(result, this.user);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void GetUserByIdNullId()
        {
            mockUserRepository.Setup(m => m.Get(this.user.Id)).Throws(new ArgumentException());
            User result = service.Get(Guid.Empty);
        }
    }
}
