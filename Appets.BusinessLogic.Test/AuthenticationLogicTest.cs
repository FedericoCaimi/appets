using Appets.DataAccess.Interface;
using Appets.Domain;
using Appets.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Appets.BusinessLogic.Test
{
    [TestClass]
    public class AuthenticationLogicTest
    {
        private User user;
        private Session session;
        private Mock<IRepository<Session>> mockSessionRepository;
        private Mock<IUserRepository> mockUserRepository;
        private AuthenticationLogic service;

        [TestInitialize]
        public void SetUp()
        {
            user = new User
            {
                Id = new Guid("9A515BC6-6B6F-4D70-1A0B-08D88E834F3F"),
                Email = "juan@gmail.com",
                UserName = "jperez",
                Password = "perez123",
                Phone = "099999999",
                Pets = new List<Pet>(),
                Post = new List<Post>()
            };

            session = new Session
            {
                UserId = user.Id,
                DateLogged = DateTime.Now,
            };

            mockSessionRepository = new Mock<IRepository<Session>>(MockBehavior.Strict);
            mockUserRepository = new Mock<IUserRepository>(MockBehavior.Strict);
            service = new AuthenticationLogic(mockSessionRepository.Object, mockUserRepository.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectParamException))]
        public void LoginUserNullEmail()
        {
            user.Email = null;
            service.Login(user);
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectParamException))]
        public void LoginUserNullPassword()
        {
            user.Password = null;
            service.Login(user);
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectLoginException))]
        public void LoginUserWrongEmail()
        {
            mockUserRepository.Setup(m => m.GetByEmail(It.IsAny<string>())).Returns(user);
            var newUser = new User
            {
                Id = new Guid("9A515BC6-6B6F-4D70-1A0B-08D88E834F3F"),
                Email = "juan",
                UserName = "jperez",
                Password = "perez123",
                Phone = "099999999",
                Pets = new List<Pet>(),
                Post = new List<Post>()
            };
            service.Login(newUser);
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectLoginException))]
        public void LoginUserWrongPassword()
        {
            mockUserRepository.Setup(m => m.GetByEmail(It.IsAny<string>())).Returns(user);
            var newUser = new User
            {
                Id = new Guid("9A515BC6-6B6F-4D70-1A0B-08D88E834F3F"),
                Email = "juan@gmail.com",
                UserName = "jperez",
                Password = "aaaa",
                Phone = "099999999",
                Pets = new List<Pet>(),
                Post = new List<Post>()
            };
            service.Login(newUser);
        }

        [TestMethod]
        public void SuccessfulLogin()
        {
            mockUserRepository.Setup(m => m.GetByEmail(It.IsAny<string>())).Returns(user);
            mockSessionRepository.Setup(m => m.Add(It.IsAny<Session>()));
            mockSessionRepository.Setup(m => m.Save());

            var newSession = service.Login(user);

            mockUserRepository.VerifyAll();
            mockSessionRepository.VerifyAll();

            Assert.AreEqual(session.UserId, newSession.UserId);
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectParamException))]
        public void LogoutEmptyGuid()
        {
            service.Logout(new Guid());
        }

        [TestMethod]
        public void Logout()
        {
            mockSessionRepository.Setup(m => m.Get(It.IsAny<Guid>())).Returns(session);
            mockSessionRepository.Setup(m => m.Remove(It.IsAny<Session>()));
            mockSessionRepository.Setup(m => m.Save());
            service.Logout(session.UserId);
            mockSessionRepository.VerifyAll();
        }

        [TestMethod]
        [ExpectedException(typeof(IncorrectParamException))]
        public void GetEmptyGuid()
        {
            service.Get(new Guid());
        }

        [TestMethod]
        public void Get()
        {
            mockSessionRepository.Setup(m => m.Get(It.IsAny<Guid>())).Returns(session);
            var newSession = service.Get(session.UserId);

            mockSessionRepository.VerifyAll();

            Assert.AreEqual(session.UserId, newSession.UserId);
        }

        [TestMethod]
        public void GetAll()
        {
            mockSessionRepository.Setup(m => m.GetAll()).Returns(new List<Session> { session });
            var allSessions = service.GetAll();

            mockSessionRepository.VerifyAll();

            Assert.AreEqual(session.UserId, allSessions.FirstOrDefault().UserId);
        }

        [TestMethod]
        public void IsLoggedIn()
        {
            mockSessionRepository.Setup(m => m.Get(It.IsAny<Guid>())).Returns(session);
            var isLoggedIn = service.IsLoggedIn(session.UserId);

            mockSessionRepository.VerifyAll();

            Assert.IsTrue(isLoggedIn);
        }
    }
}
