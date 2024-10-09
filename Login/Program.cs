using Login.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar la cadena de conexión a la base de datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Agregar contexto de la base de datos
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Agregar filtro de excepciones para desarrolladores
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configurar Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Cambia a true si quieres confirmar cuentas por email
    options.Password.RequireNonAlphanumeric = false; // Personaliza los requisitos de la contraseña
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// Agregar controladores y vistas
builder.Services.AddControllersWithViews();

// Configuración de las cookies de autenticación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Ruta para el login
    options.AccessDeniedPath = "/Account/AccessDenied"; // Ruta si no se tiene acceso
});

var app = builder.Build();

// Configurar el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); // Asegúrate de que UseAuthentication esté antes de UseAuthorization
app.UseAuthorization();

// Configurar las rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages(); // Asegúrate de incluir esto para que Identity pueda mapear las páginas de login y registro

app.Run();
