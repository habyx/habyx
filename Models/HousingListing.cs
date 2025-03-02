using System;
using System.ComponentModel.DataAnnotations;

namespace habyx.Models
{
    public class HousingListing
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty; // Add default value
        
        [Required]
        public string Description { get; set; } = string.Empty; // Add default value
        
        [Required]
        [Range(0, 100000)]
        public decimal Price { get; set; }
        
        [Required]
        public string Location { get; set; } = string.Empty; // Add default value
        
        [Range(1, 10)]
        public int Bedrooms { get; set; }
        
        [Range(1, 10)]
        public int Bathrooms { get; set; }
        
        public bool IsAvailable { get; set; } = true;
        
        public DateTime AvailableFrom { get; set; } = DateTime.UtcNow; // Default to current date
        
        public string? ImageUrls { get; set; } // Make nullable with ?
        
        public int OwnerId { get; set; }
        public User? Owner { get; set; } // Make nullable with ?
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        
        // Additional amenities
        public bool HasWifi { get; set; }
        public bool HasParking { get; set; }
        public bool IsFurnished { get; set; }
        public bool UtilitiesIncluded { get; set; }
    }
}