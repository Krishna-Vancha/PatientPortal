using PatientPortal.Models.DomainModels;
using PatientPortal.Models.IdentityEntityModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PatientPortal.Models.DTO
{
    public class PatientDto
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage ="Full name is Required")]
        public string FullName { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1900-01-01", "9999-12-31", ErrorMessage = "Date of Birth cannot be in future")]
        public DateTime DateOfBirth { get; set; }
        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }

        
        public Guid? ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public ICollection<Appointment>? Appointments { get; set; }

    }
}
