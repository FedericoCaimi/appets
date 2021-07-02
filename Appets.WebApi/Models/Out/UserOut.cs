using System;
using Appets.Domain;
using System.Collections.Generic;

namespace Appets.WebApi.Models
{
    public class UserOut
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public List<Pet> Pets { get; set; }
        public List<Post> Posts { get; set; }
        public bool IsDeleted { get; set; }

        public UserOut(User user)
        {
            Id = user.Id;
            UserName = user.UserName;
            Email = user.Email;
            Phone = user.Phone;
            Pets = user.Pets;
            Posts = user.Post;
            IsDeleted = user.IsDeleted;
        }

    }
}