using System;
using System.Collections.Generic;
using Appets.Domain;
using Appets.BusinessLogic.Interface;
using Appets.DataAccess.Interface;
using System.Linq;
using Appets.Exceptions;

namespace Appets.BusinessLogic
{
    public class UserLogic : IUserLogic
    {
        private IUserRepository Repository;
        public UserLogic(IUserRepository repository)
        {
            this.Repository = repository;
        }

        public User Create(User user)
        {
            User newUser = new User()
            {
                UserName = user.UserName,
                Password = user.Password,
                Email = user.Email,
                Phone = user.Phone,
                Pets = new List<Pet>(),
                Post = new List<Post>(),
            };
            Repository.Add(newUser);
            Repository.Save();
            return newUser;
        }

        public User Get(Guid id)
        {
            return Repository.Get(id);
        }

        public IEnumerable<User> GetAll() => Repository.GetAll();

        public void Remove(Guid id)
        {
            User userFinded = Repository.Get(id);
            Repository.Remove(userFinded);
            Repository.Save();
        }

        public User Update(Guid id, User user)
        {
            if (id != user.Id) throw new IncorrectParamException("Id and object id doesnt match");

            User userToUpdate = this.Repository.Get(id);

            userToUpdate.UserName = user.UserName;
            if(!String.IsNullOrEmpty(user.Password)){
                userToUpdate.Email = user.Email;
                userToUpdate.Password = user.Password;
            }
            userToUpdate.Phone = user.Phone;

            Repository.Update(userToUpdate);
            Repository.Save();
            return user;
        }
    }
}