using COROLA_RENTACAR.BusinessLayer.Abstract;
using COROLA_RENTACAR.BusinessLayer.Concrete;
using COROLA_RENTACAR.BusinessLayer.Mapping;
using COROLA_RENTACAR.BusinessLayer.ValidationRules;
using COROLA_RENTACAR.DataAccessLayer.Abstract;
using COROLA_RENTACAR.DataAccessLayer.Concrete;
using COROLA_RENTACAR.DataAccessLayer.EntityFramework;
using COROLA_RENTACAR.EntityLayer.Entities;
using COROLA_RENTACAR.WebUI.Models;
using COROLA_RENTACAR.WebUI.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddScoped<IPdfReportService, PdfReportService>();

builder.Services.AddDbContext<CorolaContext>();

builder.Services.AddScoped<IBrandService, BrandManager>();
builder.Services.AddScoped<IBrandDal, EfBrandDal>();

builder.Services.AddScoped<ICarService, CarManager>();
builder.Services.AddScoped<ICarDal, EfCarDal>();

builder.Services.AddScoped<ICategoryService, CategoryManager>();
builder.Services.AddScoped<ICategoryDal, EfCategoryDal>();

builder.Services.AddScoped<ILocationService, LocationManager>();
builder.Services.AddScoped<ILocationDal, EfLocationDal>();

builder.Services.AddScoped<ICustomerService, CustomerManager>();
builder.Services.AddScoped<ICustomerDal, EfCustomerDal>();

builder.Services.AddScoped<ICarImageService, CarImageManager>();
builder.Services.AddScoped<ICarImageDal, EfCarImageDal>();

builder.Services.AddScoped<IReservationService, ReservationManager>();
builder.Services.AddScoped<IReservationDal, EfReservationDal>();

builder.Services.AddAutoMapper(typeof(GenericMapping));

builder.Services.AddScoped<IValidator<Brand>, BrandValidator>();
builder.Services.AddScoped<IValidator<Category>, CategoryValidator>();
builder.Services.AddScoped<IValidator<Location>, LocationValidator>();
builder.Services.AddScoped<IValidator<Car>, CarValidator>();
builder.Services.AddScoped<IValidator<CarImage>, CarImageValidator>();
builder.Services.AddScoped<IValidator<Customer>, CustomerValidator>();
builder.Services.AddScoped<IValidator<Reservation>, ReservationValidator>();

builder.Services.AddHttpClient<IAiDriverLicenseVerificationService, OpenAiDriverLicenseVerificationService>();

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddScoped<IEmailNotificationService, MailKitEmailNotificationService>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "Corola.Admin.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.LogoutPath = "/Account/Logout";

        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Admin");
    });
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;

    if (!string.IsNullOrWhiteSpace(path) &&
        path.StartsWith("/Admin", StringComparison.OrdinalIgnoreCase))
    {
        var isAuthenticated = context.User.Identity?.IsAuthenticated == true;
        var isAdmin = context.User.IsInRole("Admin");

        if (!isAuthenticated)
        {
            var returnUrl = Uri.EscapeDataString(context.Request.Path + context.Request.QueryString);
            context.Response.Redirect($"/Account/Login?returnUrl={returnUrl}");
            return;
        }

        if (!isAdmin)
        {
            context.Response.Redirect("/Account/AccessDenied");
            return;
        }
    }

    await next();
});

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();