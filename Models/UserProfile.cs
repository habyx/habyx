using System;
using System.ComponentModel.DataAnnotations;

namespace habyx.Models
{
    public class UserProfile
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        [StringLength(500)]
        public string? Bio { get; set; }

        public string? ProfileImagePath { get; set; }
        public string? Location { get; set; }

        // Dating app features
        public string? Gender { get; set; }
        public string? InterestedIn { get; set; }

        // LinkedIn features
        [StringLength(100)]
        public string? Occupation { get; set; }
        public string? Skills { get; set; }
        public string? Education { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}