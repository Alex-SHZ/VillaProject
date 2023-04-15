using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VillaIdentity;
using VillaIdentity.Data;
using VillaIdentity.Data.DbInitializer;
using VillaIdentity.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

#region Database
builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnetion"));
});

builder.Services.AddScoped<IDbInitializer, DbInitializer>();
#endregion

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.AddRazorPages();

builder.Services.AddScoped<IProfileService, ProfileService>();

builder.Services.AddIdentityServer(options =>
{
    options.Events.RaiseErrorEvents = true;
    options.Events.RaiseInformationEvents = true;
    options.Events.RaiseFailureEvents = true;
    options.Events.RaiseSuccessEvents = true;
    options.EmitStaticAudienceClaim = true;
}).AddInMemoryIdentityResources(StaticDetails.IdentityResources)
.AddInMemoryApiScopes(StaticDetails.ApiScopes)
.AddInMemoryClients(StaticDetails.Cleints).AddAspNetIdentity<ApplicationUser>()
.AddDeveloperSigningCredential().AddProfileService<ProfileService>();






var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
SeedDatabase();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


void SeedDatabase()
{
    using (IServiceScope scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}
