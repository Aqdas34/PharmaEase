using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealProject.Data;
using RealProject.Hubs;
using RealProject.Models;
using RealProject.Models.DapperGenericRepo;

var builder = WebApplication.CreateBuilder(args);


//builder.Services.AddScoped<IRepository<Product>, ProductRepository>();
builder.Services.AddScoped<IRepository<Product>,GenericRepository<Product>>();
builder.Services.AddScoped<IRepository<Contact>, GenericRepository<Contact>>();
builder.Services.AddScoped<IRepository<BillingDetails>,GenericRepository<BillingDetails>>();
builder.Services.AddScoped<IRepository<Order>, GenericRepository<Order>>();
builder.Services.AddSignalR();


// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<MyAppUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();



// policy
builder.Services.AddAuthorization(
    options => {
        options.AddPolicy("BusinessPolicy", policy =>
        policy.RequireAssertion(context => DateTime.Now.Hour > 9 && DateTime.Now.Hour <= 24)
        );


        options.AddPolicy("AdminPolicy",
            policy => policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Admin"));
        }
    );


builder.Services.AddControllersWithViews();

// sessions
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(
    options =>
    {
        //options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
        options.Cookie.HttpOnly = true; // Make the session cookie HttpOnly
        options.Cookie.IsEssential = true; // Make the session cookie essential
    });




var app = builder.Build();

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

app.UseSession();


app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


app.MapHub<ChatHub>("/chatHub");
app.MapHub<MedicineHub>("/medicineHub");
app.Run();
