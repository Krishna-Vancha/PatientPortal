using System.ComponentModel.DataAnnotations;
namespace PatientPortal.Models.DomainModels
{
    public class Appointment
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public Patient? Patient { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string DoctorName { get; set; }
    }
}
