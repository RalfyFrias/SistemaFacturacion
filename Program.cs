using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SistemaFacturacion.Data;
using SistemaFacturacion.Services;
using Radzen;
using SistemaFacturacion.Components;
using SistemaFacturacion.Components.Account;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace SistemaFacturacion
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // Configuración de autenticación y autorización
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityUserAccessor>();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = IdentityConstants.ApplicationScheme;
                options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();

            // Configuración de la base de datos
            var connectionString = builder.Configuration.GetConnectionString("ConStr") ??
                throw new InvalidOperationException("Connection string 'ConStr' not found.");

            builder.Services.AddDbContext<Contexto>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            // Configuración de Identity con roles
            builder.Services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
            })
            .AddRoles<IdentityRole>() // Habilitar soporte para roles
            .AddEntityFrameworkStores<Contexto>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

            // Configuración de políticas de autorización
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
            });

            // Configuración de cookies
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            });

            // Servicios de la aplicación
            builder.Services.AddScoped<ProductosService>();
            builder.Services.AddScoped<FacturasService>();
            builder.Services.AddScoped<FinanciamientoService>();
            builder.Services.AddScoped<CuentasXcobrarService>();
            builder.Services.AddScoped<ReparacionesService>();
            builder.Services.AddRadzenComponents();
            builder.Services.AddSingleton<NotificationService>();
            //builder.Services.AddSingleton<IEmailSender, IdentityNoOpEmailSender>();

            var app = builder.Build();

            // Crear roles y usuarios iniciales
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    await CreateRolesAndUsers(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Ocurrió un error al crear los roles y usuarios iniciales");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAntiforgery();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();

            app.Run();
        }

        private static async Task CreateRolesAndUsers(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

            // Crear roles si no existen
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                    if (result.Succeeded)
                    {
                        logger.LogInformation($"Rol {roleName} creado exitosamente");
                    }
                    else
                    {
                        logger.LogError($"Error al crear el rol {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }

            // Crear usuario Admin
            var adminUser = new ApplicationUser
            {
                UserName = "Admin@gmail.com",
                Email = "Admin@gmail.com",
                EmailConfirmed = true
            };

            string adminPassword = "Admin123@";
            var admin = await userManager.FindByEmailAsync(adminUser.Email);
            if (admin == null)
            {
                var createAdmin = await userManager.CreateAsync(adminUser, adminPassword);
                if (createAdmin.Succeeded)
                {
                    logger.LogInformation("Usuario admin creado exitosamente");

                    var roleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                    if (roleResult.Succeeded)
                    {
                        logger.LogInformation("Rol Admin asignado al usuario admin");
                    }
                    else
                    {
                        logger.LogError($"Error al asignar rol: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    logger.LogError($"Error al crear usuario admin: {string.Join(", ", createAdmin.Errors.Select(e => e.Description))}");
                }
            }

            // Crear usuario normal
            var normalUser = new ApplicationUser
            {
                UserName = "user@gmail.com",
                Email = "user@gmail.com",
                EmailConfirmed = true
            };

            string userPassword = "User123@";
            var user = await userManager.FindByEmailAsync(normalUser.Email);
            if (user == null)
            {
                var createUser = await userManager.CreateAsync(normalUser, userPassword);
                if (createUser.Succeeded)
                {
                    logger.LogInformation("Usuario normal creado exitosamente");

                    var roleResult = await userManager.AddToRoleAsync(normalUser, "User");
                    if (roleResult.Succeeded)
                    {
                        logger.LogInformation("Rol User asignado al usuario normal");
                    }
                    else
                    {
                        logger.LogError($"Error al asignar rol: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    logger.LogError($"Error al crear usuario normal: {string.Join(", ", createUser.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}