using System;
using System.ComponentModel.DataAnnotations;

namespace habyx.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        [Required]
        public string Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public virtual UserProfile Sender { get; set; }
        public virtual UserProfile Receiver { get; set; }
    }
}