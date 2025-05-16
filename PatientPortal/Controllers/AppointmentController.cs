using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts;
using PatientPortal.Data;
using PatientPortal.Models.DomainModels;
namespace PatientPortal.Controllers
{
    public class AppointmentController : Controller
    {
        private readonly PatientPortalDbContext dbContext;
        public AppointmentController(PatientPortalDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public IActionResult Book(Guid id)
        {
            var patient = dbContext.Patients.Find(id);
            if (patient == null)
            {
                return NotFound();
            }
            ViewBag.PatientId = patient.Id;
            ViewBag.PatientName = patient.FullName;
            return View(new Appointment { PatientId=patient.Id,Patient=patient });
        }
        [HttpPost]
        public IActionResult Book(Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointment.Id = Guid.NewGuid();
                dbContext.Appointments.Add(appointment);
                dbContext.SaveChanges();
                return RedirectToAction("Details","Patient",new {id=appointment.PatientId });
            }
            var patient = dbContext.Patients.Find(appointment.PatientId);
            ViewBag.PatientName = patient?.FullName ?? "Unknown";
            return View(appointment);
        }
        [HttpGet]
        public IActionResult List(Guid id) //Patient Id
        { 
            var patient=dbContext.Patients.Include(p=>p.Appointments)
                .FirstOrDefault(p=>p.Id==id);
            if (patient == null)
            { return NotFound();
            }
            ViewBag.PatientName= patient.FullName;
            ViewBag.PatientId = patient.Id;
            return View(patient.Appointments.ToList());
        }
        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var appointment = dbContext.Appointments.Include(p => p.Patient).FirstOrDefault(a => a.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }
        [HttpPost]
        public IActionResult Edit(Appointment appointment)
        {
            if(ModelState.IsValid)
            {
                dbContext.Appointments.Update(appointment);
                dbContext.SaveChanges();
                return RedirectToAction("List", new { id = appointment.PatientId });
                //return RedirectToAction("List","Appointment",new { id=appointment.PatientId});
            }
            return View(appointment);
        }
        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            var appointment = dbContext.Appointments.Include(a=>a.Patient).FirstOrDefault(a=>a.Id==id);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(Guid id) {
            var appointment = dbContext.Appointments.Find(id);
            if (appointment == null)
            { return NotFound();
            }
            var patientId=appointment.PatientId;
            dbContext.Appointments.Remove(appointment);
            dbContext.SaveChanges();
            return RedirectToAction("List","Appointment", new {id=patientId });
        }
    }
}
