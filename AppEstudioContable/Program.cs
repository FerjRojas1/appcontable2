using ServiciosEC.Interfaces;
using ServiciosEC.Managers;
using Microsoft.AspNetCore.Authentication.Cookies;
using ServiciosEC.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ServiciosEC.Utilidades;
using ServiciosEC.Middleware;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using ServiciosEC.Interfaces.Managers;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.

builder.Services.AddControllersWithViews(options =>
{
    
    options.ModelBindingMessageProvider.SetValueMustBeANumberAccessor(
        _ => "El valor ingresado debe ser un número válido.");
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

//builder.Services.AddScoped<EstadoManager>();
//builder.Services.AddScoped<PersonaManager>();

//dbContext de EntityFramework
//builder.Services.AddDbContext<ECContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("local"),
//    sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
//    ));

//builder.Services.AddDbContext<ECContext>((serviceProvider, options) =>
//{
//    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
//    var connectionString = configuration.GetConnectionString("local");

//    options.UseSqlServer(connectionString, sqlOptions =>
//    {
//        sqlOptions.EnableRetryOnFailure();
//    });

//});
builder.Services.AddDbContext<ECContext>((serviceProvider, options) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection");

    var auditoriaInterceptor = serviceProvider.GetRequiredService<AuditoriaInterceptor>();

    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure();
    });

    
    options.AddInterceptors(auditoriaInterceptor);
});


//inyeccion de dependencias
builder.Services.AddScoped<EstadoManager>();
builder.Services.AddScoped<PersonaManager>();
builder.Services.AddScoped<IUsuariosManager, UsuarioManager>();
builder.Services.AddScoped<IClienteManager, ClienteManager>();
builder.Services.AddScoped<IVentaManager, VentaManager>();
builder.Services.AddScoped<ICompraManager, CompraManager>();
builder.Services.AddScoped<IIvaManager, IvaManager>();
builder.Services.AddScoped<ILibroIvaManager, LibroIvaManager>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IExcelDataHandler, ExcelDataHandler>();

builder.Services.AddScoped<AuditoriaInterceptor>(); // interceptor 
builder.Services.AddScoped<IAuditoria, AuditoriaService>(); // servicio de auditoria 

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
{
    //si el usuario no esta autenticado sera dirigido aca
    options.LoginPath = "/Login";

    //si el usuario no es admin sera dirigido aca
    options.AccessDeniedPath = "/Home/Error403";
});

// Configuración de la política de autorización
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

//  Aplicar migraciones automáticamente al arrancar
//if (!app.Environment.IsDevelopment())

    //using (var scope = app.Services.CreateScope())
    //{
    //    var db = scope.ServiceProvider.GetRequiredService<ECContext>();
    //    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    //    try
    //    {
    //        logger.LogInformation("Aplicando migraciones...");
    //        db.Database.Migrate();
    //        logger.LogInformation("Migraciones aplicadas correctamente.");
    //    }
    //    catch (Exception ex)
    //    {
    //        logger.LogError(ex, "Error al aplicar migraciones: {Message}", ex.Message);
    //        throw;
    //    }
    //}

//else
//{
//    // En desarrollo, solo logueamos
//    Console.WriteLine("Entorno de desarrollo: omitiendo migraciones automáticas.");
//}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


var defaultCulture = new CultureInfo("en-US"); 
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = new List<CultureInfo> { defaultCulture },
    SupportedUICultures = new List<CultureInfo> { defaultCulture }
};
app.UseRequestLocalization(localizationOptions); 

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();