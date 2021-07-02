using System;

namespace Appets.Domain
{
    public class Session : DomainEntity
    {
        public Guid UserId { get; set; }
        public DateTime DateLogged { get; set; }
        public Session() : base() { }
    }
}