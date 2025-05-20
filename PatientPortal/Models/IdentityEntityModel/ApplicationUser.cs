using Microsoft.AspNetCore.Identity;
using PatientPortal.Models.DomainModels;
using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Models.IdentityEntityModel
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [Required]
        public string FullName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public string Gender { get; set; }

        public string Address { get; set; }

        public string ProfileImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Optional flags
        public bool IsPatient { get; set; } = false;

        public bool IsAdmin { get; set; } = false;

        // Optional navigation property (1-to-1 with Patient)
        public Patient PatientProfile { get; set; }

    }
}