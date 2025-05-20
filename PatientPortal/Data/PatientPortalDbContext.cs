using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Models.DomainModels;
using PatientPortal.Models.IdentityEntityModel;

namespace PatientPortal.Data
{
    public class PatientPortalDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public PatientPortalDbContext(DbContextOptions<PatientPortalDbContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
    }
}
