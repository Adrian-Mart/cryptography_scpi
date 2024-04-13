using Microsoft.EntityFrameworkCore;
using scpi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add the database context to the services
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add the http context accessor to the services
builder.Services.AddHttpContextAccessor();

// build the application
var app = builder.Build();
// If the application is in development mode
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Use the https redirection
app.UseHttpsRedirection();
// Use the static files
app.UseStaticFiles();
// Use the routing
app.UseRouting();
// Use the authorization
app.UseAuthorization();
// Use the razor mapping
app.MapRazorPages();

// Run the application
app.Run();
