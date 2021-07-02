using System;

namespace Appets.Domain
{
    public abstract class DomainEntity
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set;}
    }
}
