using System;

namespace Appets.Domain
{
    public class Tag : DomainEntity
    {
        public string Type { get; set; }
        public string Value { get; set; }

        public override bool Equals(object obj)
        {
            var tag = obj as Tag;
            return tag != null &&
                   Type == tag.Type;
        }
    }
}