using System;
using System.Collections.Generic;
using Appets.Domain;

namespace Appets.WebApi.Models
{
    public class PetIn
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Type { get; set; }
        public List<Tag> Tags { get; set; }
        public bool IsDeleted { get; set; }

        public PetIn()
        {
        }

        public Pet ToEntity() => new Pet()
        {
            Id = this.Id,
            Name = this.Name,
            Image = this.Image,
            Type = this.Type,
            Tags = this.Tags,
            IsDeleted = this.IsDeleted
        };

    }
}