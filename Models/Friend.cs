using System;

namespace habyx.Models
{
    public class Friend
    {
        public int Id { get; set; }
        public int RequesterId { get; set; }
        public int AddresseeId { get; set; }
        public FriendStatus Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        public virtual UserProfile Requester { get; set; }
        public virtual UserProfile Addressee { get; set; }
    }

    public enum FriendStatus
    {
        Pending,
        Accepted,
        Rejected,
        Blocked
    }
}