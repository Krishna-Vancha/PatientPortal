using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PatientPortal.Data;
using PatientPortal.Models.IdentityEntityModel;
using Serilog;
using System.Text;
var builder = WebApplication.CreateBuilder(args);
// Logging
var logger = new LoggerConfiguration().
    WriteTo.Console()
    .MinimumLevel.Information().CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);
// Add MVC
builder.Services.AddControllersWithViews();
// Add DB Context
builder.Services.AddDbContext<PatientPortalDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PatientConnectionString")));
// Configure Authentication
//Enable Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>().AddDefaultTokenProviders()
    .AddEntityFrameworkStores<PatientPortalDbContext>().
    AddUserStore<UserStore<ApplicationUser, ApplicationRole, PatientPortalDbContext, Guid>>().
    AddRoleStore<RoleStore<ApplicationRole, PatientPortalDbContext,Guid>>(); 


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
.AddOpenIdConnect("AzureAd", options =>
{
    options.Authority = $"{builder.Configuration["AzureAd:Instance"]}{builder.Configuration["AzureAd:TenantId"]}/v2.0";
    options.ClientId = builder.Configuration["AzureAd:ClientId"];
    options.ClientSecret = builder.Configuration["AzureAd:ClientSecret"];
    options.CallbackPath = builder.Configuration["AzureAd:CallbackPath"];
    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.GetClaimsFromUserInfoEndpoint = true;
});
// Authorization
builder.Services.AddAuthorization();
var app = builder.Build();
// Middleware
if(!app.Environment.IsDevelopment())
{
   app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();  // 🔐 Authenticate
app.UseAuthorization();   // ✅ Authorize
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Patient}/{action=Login}/{id?}");
app.Run();
