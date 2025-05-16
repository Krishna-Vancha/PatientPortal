using System.ComponentModel.DataAnnotations;
namespace PatientPortal.Models.DomainModels
{
    public class Patient
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Full name is required.")]
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
        public ICollection<Appointment>? Appointments { get; set; }
    }
}
