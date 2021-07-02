using System;
using System.Collections.Generic;

namespace Appets.Domain
{
    public class Pet : DomainEntity
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string Type { get; set; }
        public List<Tag> Tags { get; set; }
        public User Owner { get; set; }
        public List<Post> Posts { get; set; }


        public override bool Equals(object obj)
        {
            var pet = obj as Pet;
            return pet != null &&
                   Name == pet.Name &&
                   Owner == pet.Owner;
        }
    }
}