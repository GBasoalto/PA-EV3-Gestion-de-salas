using Gestion_de_salas.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


//SE AGREGA LOGIN 
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // a los 30 minutos de inactividad se cierra la sesión
    options.Cookie.HttpOnly = true; // Seguridad: solo accesible por el servidor
    options.Cookie.IsEssential = true; // Necesario para el funcionamiento de la aplicación (cookies esenciales)
});

//CONSTRUIR LA APP DESPUES DE CONFIGURAR SERVICIOS
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

app.UseRouting();

app.UseSession(); // Habilitar el uso de sesiones de login

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    //pattern: "{controller=Home}/{action=Index}/{id?}");
    pattern: "{controller=Login}/{action=index}/{id?}"); //nombre del controlador y la vista


//REDIRECCION AL LOGIN AL INICIAR LA APLICACION
app.MapGet("/", context =>
{
    context.Response.Redirect("/Login");
    return Task.CompletedTask;
});


app.Run();
