using System;
using System.Collections.Generic;
using Appets.Domain;
using Appets.DataAccess.Interface;
using Appets.BusinessLogic.Interface;
using Appets.Exceptions;

namespace Appets.BusinessLogic
{
    public class AuthenticationLogic : IAuthenticationLogic
    {
        private IRepository<Session> SessionRepository { get; set; }
        private IUserRepository UserRepository { get; set; }

        public AuthenticationLogic(IRepository<Session> sessionRepository)
        {
            this.SessionRepository = sessionRepository;
        }

        public AuthenticationLogic(IRepository<Session> sessionRepository, IUserRepository adminRepository)
        {
            this.SessionRepository = sessionRepository;
            this.UserRepository = adminRepository;
        }

        public Session Login(User user)
        {
            if (user.Email == null || user.Password == null)
            {
                throw new IncorrectParamException("Email and password is needed to login");
            }
            var DBuser = UserRepository.GetByEmail(user.Email);
            if (!isCorrectLogin(DBuser, user))
            {
                throw new IncorrectLoginException();
            }
            var newSession = new Session
            {
                UserId = DBuser.Id,
                DateLogged = DateTime.Now,
            };

            SessionRepository.Add(newSession);
            SessionRepository.Save();
            return newSession;
        }

        private bool isCorrectLogin(User DBuser, User user)
        {
            return (DBuser.Email == user.Email && DBuser.Password == user.Password);
        }

        public void Logout(Guid token)
        {
            if (token == Guid.Empty)
            {
                throw new IncorrectParamException("Invalid token");
            }
            var session = SessionRepository.Get(token);
            SessionRepository.Remove(session);
            SessionRepository.Save();
        }

        public IEnumerable<Session> GetAll() => SessionRepository.GetAll();

        public Session Get(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new IncorrectParamException("Invalid id");
            }                
            return SessionRepository.Get(id);
        }

        public bool IsLoggedIn(Guid token)
        {
            var session = SessionRepository.Get(token);
            var loggedOutToken = session.IsDeleted;
            var dueDate = session.DateLogged.AddHours(24);
            var validToken = DateTime.Now < dueDate;
            return !loggedOutToken && validToken;
        }
    }
}