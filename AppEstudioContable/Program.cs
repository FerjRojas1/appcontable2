using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using ServiciosEC.Interfaces;
using ServiciosEC.Interfaces.Managers;
using ServiciosEC.Managers;
using ServiciosEC.Middleware;
using ServiciosEC.Models;
using ServiciosEC.Utilidades;
using System.Globalization;

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

// DbContext
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

// Inyección de dependencias
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
builder.Services.AddScoped<AuditoriaInterceptor>();
builder.Services.AddScoped<IAuditoria, AuditoriaService>();

// Autenticación
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/Home/Error403";
    });

// Autorización
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var app = builder.Build();

// ==================== MIGRACIONES AUTOMÁTICAS ====================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ECContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        logger.LogInformation("Aplicando migraciones...");
        db.Database.Migrate();
        logger.LogInformation("Migraciones aplicadas correctamente.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error al aplicar migraciones: {Message}", ex.Message);
        throw; // Si querés que la app arranque igual, borrá este throw
    }
}

// ==================== PIPELINE HTTP ====================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            logger.LogError(exception, "Error no manejado en {Path}", context.Request.Path);

            context.Response.Redirect("/Home/Error");
        });
    });
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// ==================== CULTURA ====================
var defaultCulture = new CultureInfo("es-AR");
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