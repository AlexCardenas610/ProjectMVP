using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PlayNexus.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add configuration from appsettings.Smtp.json
builder.Configuration.AddJsonFile("appsettings.Smtp.json", optional: true, reloadOnChange: true);

builder.Services.AddRazorPages();

// Register the database context and Identity services
builder.Services.AddDbContext<PlayNexusDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<PlayNexusDbContext>()
.AddDefaultTokenProviders();

builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 104857600; // 100 MB
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
