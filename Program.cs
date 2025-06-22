using Alzheimer_Detection.Data;
using Alzheimer_Detection.Services;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// 1. Services MVC
builder.Services.AddControllersWithViews();

// 2. Base de données SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Authentification Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

// 4. Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 5. PDF (DinkToPdf)
builder.Services.AddSingleton<IConverter>(new SynchronizedConverter(new PdfTools()));
builder.Services.AddScoped<IPdfReportService, PdfReportService>();

// 6. Journalisation personnalisée (console + debug + fichier)
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders();
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();

    // Création du dossier de logs
    var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
    Directory.CreateDirectory(logDirectory);

    // Ajout du logger fichier personnalisé
    loggingBuilder.AddProvider(new FileLoggerProvider(
        Path.Combine(logDirectory, $"alzheimer-log-{DateTime.Now:yyyy-MM-dd}.log")
    ));
});

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
