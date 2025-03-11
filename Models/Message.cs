using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace habyx.Models
{
    public class Message
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public int SenderId { get; set; }

        [ForeignKey("SenderId")]
        public User Sender { get; set; } = null!;

        [Required]
        public int ReceiverId { get; set; }

        [ForeignKey("ReceiverId")]
        public User Receiver { get; set; } = null!;

        // Add these properties
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;

        public Message() { }

        public Message(string content, int senderId, int receiverId)
        {
            Content = content;
            SenderId = senderId;
            ReceiverId = receiverId;
        }
    }
}