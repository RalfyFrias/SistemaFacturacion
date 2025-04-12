using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SistemaFacturacion.Data;
using SistemaFacturacion.Models;
using SistemaFacturacion.Models;

namespace SistemaFacturacion.Data
{
    public class Contexto : IdentityDbContext<ApplicationUser>
    {
        public Contexto(DbContextOptions<Contexto> options) : base(options) { }

        public DbSet<Productos> Productos { get; set; }
        public DbSet<Ventas> Facturas { get; set; }
        public DbSet<Financiamiento> Financiamientos { get; set; }

        public DbSet<Reparaciones> Reparaciones { get; set; }
        public DbSet<VentasDetalle> FacturaDetalle { get; set; }
        public DbSet<CuentasXcobrar> CuentasXcobrar { get; set; }
    }
}
