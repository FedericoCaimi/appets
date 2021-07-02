using System;
using System.Collections.Generic;
using Appets.Domain;
using Appets.Domain.Enums;

namespace Appets.WebApi.Models
{
    public class PostIn
    {
        public Guid Id { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Ubication { get; set; }
        public string ReporterPhone { get; set; }
        public PostTypeEnum Type { get; set; }
        public List<Tag> Tags { get; set; }
        public bool IsDeleted { get; set; }

        public PostIn()
        {
        }

        public Post ToEntity() => new Post()
        {
            Id = this.Id,
            Image = this.Image,
            Title = this.Title,
            Description = this.Description,
            Ubication = this.Ubication,
            ReporterPhone = this.ReporterPhone,
            Type = this.Type,
            Tags = this.Tags,
            IsDeleted = this.IsDeleted,
        };

    }
}