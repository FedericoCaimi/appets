using System;
using Appets.Domain;
using System.Collections.Generic;
using Appets.Domain.Enums;

namespace Appets.WebApi.Models
{
    public class SimilarPostOut
    {
        public Guid postId { get; set; }
        public string postTitle { get; set; }
        public string description { get; set; }
        public string ubication { get; set; }
        public string ReporterPhone { get; set; }
        public string postImage { get; set; }
        public List<Tag> Tags { get; set; }
        public PostTypeEnum Type { get; set; }
        public int coincidencePercentage { get; set; }

        public SimilarPostOut(Tuple<int, Post> post)
        {
            postId = post.Item2.Id;
            postTitle = post.Item2.Title;
            description = post.Item2.Description;
            ubication = post.Item2.Ubication;
            ReporterPhone = post.Item2.ReporterPhone;
            postImage = post.Item2.Image;
            Type = post.Item2.Type;
            Tags = post.Item2.Tags;
            coincidencePercentage = post.Item1;
        }

    }
}