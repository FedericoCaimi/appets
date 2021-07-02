using System;
using System.Collections.Generic;

namespace Appets.Domain
{
    public class User : DomainEntity
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public List<Pet> Pets { get; set; }
        public List<Post> Post { get; set; }

        public override bool Equals(object obj)
        {
            var user = obj as User;
            return user != null &&
                   Email == user.Email;
        }
    }
}