using System;
using System.Collections.Generic;
using Appets.Domain;

namespace Appets.WebApi.Models
{
    public class PetOut
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Image { get; set; }
        public bool IsDeleted { get; set; }
        public List<Tag> Tags { get; set; }
        public Guid OwnerId { get; set; }

        public string Phone { get; set; }
        public List<PostOut> Posts { get; set; }

        public PetOut(Pet pet)
        {
            Id = pet.Id;
            Name = pet.Name;
            Type = pet.Type;
            Image = pet.Image;
            IsDeleted = pet.IsDeleted;
            Tags = pet.Tags;
            OwnerId = pet.Owner != null ? pet.Owner.Id : Guid.Empty;
            Phone = pet.Owner != null ? pet.Owner.Phone : "";
            Posts = pet.Posts != null ? pet.Posts.ConvertAll(a => new PostOut(a)) : null;
        }

    }
}