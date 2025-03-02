using System;
using System.ComponentModel.DataAnnotations;

namespace habyx.Models
{
    public class HousingApplication
    {
        public int Id { get; set; }
        
        public int ListingId { get; set; }
        public HousingListing? Listing { get; set; }
        
        public int ApplicantId { get; set; }
        public User? Applicant { get; set; }
        
        [StringLength(500)]
        public string Message { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    }
}