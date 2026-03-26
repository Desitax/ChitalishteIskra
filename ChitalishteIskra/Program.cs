using ChitalishteIskra.Core.Contracts;
using ChitalishteIskra.Core.Services;
using ChitalishteIskra.Data;
using ChitalishteIskra.Data.Entities;
using ChitalishteIskra.Data.Seed;
using ChitalishteIskra.Data.Utilities;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ChitalishteIskraDbContext>(options =>
    options.UseSqlServer(connectionString));


builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 5;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<ChitalishteIskraDbContext>()
.AddDefaultTokenProviders();

var cloudinarySettings = builder.Configuration.GetSection("CloudinarySettings").Get<CloudinarySettings>();
builder.Services.AddSingleton<Cloudinary>((sp) =>
{
    return new Cloudinary(new Account(cloudinarySettings.CloudName, cloudinarySettings.ApiKey, cloudinarySettings.ApiSecret));
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddAuthorization();


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IBookLessonService, BookLessonService>();
builder.Services.AddScoped<ILessonService, LessonService>();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<ITeacherAvailabilityService, TeacherAvailabilityService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    await IdentitySeeder.SeedRolesAsync(roleManager);
    await IdentitySeeder.SeedAdminAsync(userManager);
    await IdentitySeeder.SeedTeacherAsync(userManager);
    await IdentitySeeder.SeedStudentAsync(userManager);
    await IdentitySeeder.SeedParentAsync(userManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
