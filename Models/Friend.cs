using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace habyx.Models
{
    public class Friend
    {
        public int Id { get; set; }

        [Required]
        public int RequesterId { get; set; }

        [ForeignKey("RequesterId")]
        public User Requester { get; set; } = null!;

        [Required]
        public int AddresseeId { get; set; }

        [ForeignKey("AddresseeId")]
        public User Addressee { get; set; } = null!;

        // Changed from enum to string
        [Required]
        public string Status { get; set; } = "Pending";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Friend() { }

        public Friend(int requesterId, int addresseeId)
        {
            RequesterId = requesterId;
            AddresseeId = addresseeId;
        }
    }
}