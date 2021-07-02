using System;
using Appets.Domain;

namespace Appets.WebApi.Models
{
    public class UserIn
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public bool IsDeleted { get; set; }

        public UserIn()
        {
        }

        public User ToEntity() => new User()
        {
            Id = this.Id,
            UserName = this.UserName,
            Email = this.Email,
            Password = this.Password,
            Phone = this.Phone,
            IsDeleted = this.IsDeleted,
        };

    }
}