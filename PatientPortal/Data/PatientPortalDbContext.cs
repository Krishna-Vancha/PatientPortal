using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PatientPortal.Models.DomainModels;
namespace PatientPortal.Data
{
    public class PatientPortalDbContext : DbContext
    {
        public PatientPortalDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Patient>  Patients{ get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
    }
}
