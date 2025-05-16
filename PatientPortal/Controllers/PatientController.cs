using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PatientPortal.Data;
using PatientPortal.Models.DomainModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
namespace PatientPortal.Controllers
{
    public class PatientController : Controller
    {
        private readonly PatientPortalDbContext dbContext;
        private readonly IConfiguration configuration;
        private readonly ILogger<PatientController> logger;
        public PatientController(PatientPortalDbContext dbContext, IConfiguration configuration, ILogger<PatientController> logger)
        {
            this.dbContext = dbContext;
            this.configuration = configuration;
            this.logger = logger;
        }
        // ---------------------- Admin Login -----------------------------------------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var admin = dbContext.Admins.FirstOrDefault(a => a.Username == username && a.Password == password);
            if (admin == null)
            {
                ViewBag.Error = "Invalid credentials";
                return View();
            }
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, admin.Username),
                new Claim(ClaimTypes.Role, "Admin")
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            Response.Cookies.Append("jwt", jwtToken, new CookieOptions { HttpOnly = true, Secure = true });
            return RedirectToAction("List");
        }
        [Authorize]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Login");
        }
        // ----------------------- Patient CRUD ------------------ //
        [Authorize]
        public IActionResult List(string searchTerm)
        {
            logger.LogInformation("Getting List Of Patients");
            var patients = dbContext.Patients.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                patients = patients.Where(p => p.FullName.Contains(searchTerm));
            }
            logger.LogInformation($"Finished getting Patients based on search:{JsonSerializer.Serialize(patients)}");
            ViewBag.SearchTerm = searchTerm;
            return View(patients.ToList());
        }
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public IActionResult Create(Patient patient)
        {
            if (ModelState.IsValid)
            {
                if (patient.DateOfBirth > DateTime.Today)
                {
                    ModelState.AddModelError("DateOfBirth", "Date of Birth cannot be in the future.");
                    return View(patient);
                }
                patient.Id = Guid.NewGuid();
                dbContext.Patients.Add(patient);
                dbContext.SaveChanges();
                return RedirectToAction("List");
            }
            return View(patient);
        }
        [Authorize]
        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var patient = dbContext.Patients.Find(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }
        [Authorize]
        [HttpPost]
        public IActionResult Edit(Patient patient)
        {
            if (ModelState.IsValid)
            {
                if (patient.DateOfBirth > DateTime.Today)
                {
                    ModelState.AddModelError("DateOfBirth", "Date of Birth cannot be in the future.");
                    return View(patient);
                }
                dbContext.Patients.Update(patient);
                dbContext.SaveChanges();
                return RedirectToAction("List");
            }
            return View(patient);
        }
        [Authorize]
        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            var patient = dbContext.Patients.Find(id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }
        [Authorize]
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(Guid id)
        {
            var patient = dbContext.Patients.Find(id);
            if (patient != null)
            {
                dbContext.Patients.Remove(patient);
                dbContext.SaveChanges();
            }
            return RedirectToAction("List");
        }
        [Authorize]
        public IActionResult Details(Guid id)
        {
            var patient = dbContext.Patients
                .Include(p => p.Appointments)
                .FirstOrDefault(p => p.Id == id);
            if (patient == null)
            {
                return NotFound();
            }
            return View(patient);
        }
    }
}
