using Appets.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Appets.Domain
{
    public class Post : DomainEntity
    {
        public string Image {get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Tag> Tags { get; set; }
        public string ReporterPhone { get; set; }
        public string Ubication { get; set; }
        public PostTypeEnum Type { get; set; }
        public string IgnoredPosts { get; set; }


        public override bool Equals(object obj)
        {
            var post = obj as Post;
            return post != null &&
                   Title == post.Title &&
                   Type == post.Type &&
                   ReporterPhone == post.ReporterPhone;
        }
    }
}