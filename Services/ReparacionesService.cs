using Microsoft.EntityFrameworkCore;
using SistemaFacturacion.Data;
using SistemaFacturacion.Models;
using System.Linq.Expressions;

namespace SistemaFacturacion.Services
{
    public class ReparacionesService
    {
        private readonly Contexto _contexto;

        public ReparacionesService(Contexto contexto)
        {
            _contexto = contexto;
        }

        // Crear o modificar una reparación
        public async Task<bool> Crear(Reparaciones reparacion)
        {
            if (!await Existe(reparacion.ReparacionesId))
                return await Insertar(reparacion);
            else
                return await Modificar(reparacion);
        }

        // Verificar si una reparación existe por su ID
        public async Task<bool> Existe(int id)
        {
            return await _contexto.Reparaciones.AnyAsync(r => r.ReparacionesId == id);
        }

        // Insertar una nueva reparación
        public async Task<bool> Insertar(Reparaciones reparacion)
        {
            _contexto.Reparaciones.Add(reparacion);
            return await _contexto.SaveChangesAsync() > 0;
        }

        // Modificar una reparación existente
        public async Task<bool> Modificar(Reparaciones reparacion)
        {
            _contexto.Update(reparacion);
            var modifico = await _contexto.SaveChangesAsync() > 0;
            _contexto.Entry(reparacion).State = EntityState.Detached;
            return modifico;
        }

        // Eliminar una reparación por ID
        public async Task<bool> Eliminar(int reparacionId)
        {
            var reparacion = await _contexto.Reparaciones.FindAsync(reparacionId);
            if (reparacion != null)
            {
                _contexto.Reparaciones.Remove(reparacion);
                await _contexto.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> EliminarDetalle(int detalleId)
        {
            var detalle = await _contexto.FacturaDetalle.FindAsync(detalleId);
            if (detalle != null)
            {
                _contexto.FacturaDetalle.Remove(detalle);
                await _contexto.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // Obtener una reparación por ID
        public async Task<Reparaciones?> ObtenerPorId(int reparacionId)
        {
            return await _contexto.Reparaciones
                .Include(v => v.VentasDetalles) // Cargar los detalles de la factura
                .FirstOrDefaultAsync(r => r.ReparacionesId == reparacionId);
        }

        public async Task<Reparaciones?> Buscar(int id)
        {
            return await _contexto.Reparaciones
                .Include(f => f.VentasDetalles)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.ReparacionesId == id);
        }


        // Buscar reparaciones por nombre de cliente
        public async Task<List<Reparaciones>> BuscarPorCliente(string nombreCliente)
        {
            return await _contexto.Reparaciones
                .Where(r => r.NombreCliente.ToLower().Contains(nombreCliente.ToLower()))
                .ToListAsync();
        }

        public async Task<bool> ExisteNombreCliente(string nombreCliente)
        {
            var reparacionesExistentes = await _contexto.Reparaciones
                .Where(r => r.NombreCliente == nombreCliente)
                .ToListAsync();

            return reparacionesExistentes.Any();
        }


        // Listar todas las reparaciones
        public async Task<List<Reparaciones>> Listar(Expression<Func<Reparaciones, bool>> criterio)
        {
            return await _contexto.Reparaciones
                .Include(f => f.VentasDetalles)
                .AsNoTracking()
                .Where(criterio)
                .ToListAsync();
        }
    }
}
