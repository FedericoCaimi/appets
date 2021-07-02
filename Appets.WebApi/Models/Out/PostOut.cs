using System;
using Appets.Domain;
using System.Collections.Generic;
using Appets.Domain.Enums;

namespace Appets.WebApi.Models
{
    public class PostOut
    {
        public Guid postId { get; set; }
        public string postTitle { get; set; }
        public string description { get; set; }
        public string ReporterPhone { get; set; }
        public string postImage { get; set; }
        public string ubication { get; set; }
        public List<Tag> Tags { get; set; }
        public PostTypeEnum Type { get; set; }

        public PostOut(Post post)
        {
            postId = post.Id;
            postTitle = post.Title;
            description = post.Description;
            ReporterPhone = post.ReporterPhone;
            postImage = post.Image;
            ubication = post.Ubication;
            Type = post.Type;
            Tags = post.Tags;
        }

    }
}