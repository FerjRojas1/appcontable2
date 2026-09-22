using Microsoft.EntityFrameworkCore;
using ServiciosEC.Interfaces.Managers;
using ServiciosEC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiciosEC.Managers
{
    public class ClienteManager : IClienteManager
    {
        private readonly ECContext _context;
        public ClienteManager(ECContext context)
        {
            _context = context;
        }

        public async Task<bool> Existe(int id, CancellationToken cancellationToken)
        {
            return await _context.Clientes.AnyAsync(e => e.IdPersona == id, cancellationToken);
        }

        public async Task<bool> ExisteCliente(Cliente cliente, CancellationToken cancellationToken)
        {
            var clienteBuscado = await _context.Clientes
                .FirstOrDefaultAsync(c =>
                    (c.Cuit == cliente.Cuit || ( c.Dni == cliente.Dni && c.Dni != null))
                    && c.IdPersona != cliente.IdPersona,
                    cancellationToken);

            return clienteBuscado != null;
        }

        public async Task<Cliente?> ObtenerClientePorCuitAsync(string cuit, CancellationToken cancellationToken)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Cuit == cuit, cancellationToken);

            return cliente;
        }
        
        public async Task Insertar(Cliente cliente, CancellationToken cancellationToken)
        {
            
            _context.Clientes.Add(cliente);

            _context.Entry(cliente).Property("Discriminator").CurrentValue = "Cliente";

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Cliente>> ObtenerTodos(CancellationToken cancellationToken)
        {
            return await _context.Clientes
                .Where(c => c.EstadoId == 1)
                .ToListAsync(cancellationToken);
        }

        public async Task<Cliente> ObtenerPorId(int id, CancellationToken cancellationToken)
        {
            return await _context.Clientes
                .Include(c => c.Estado)
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(c => c.IdPersona == id, cancellationToken);

        }

        public async Task Editar(Cliente cliente, CancellationToken cancellationToken)
        {
            
            _context.Entry(cliente).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Borrar(int id, CancellationToken cancellationToken)
        {
            var cliente = await ObtenerPorId(id,cancellationToken);
            if (cliente is not null)
            {
                cliente.EstadoId = 2;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<IEnumerable<Cliente>> ObtenerTodosAsync(CancellationToken cancellationToken) 
        {
            return await _context.Clientes 
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Cliente>> ObtenerInactivosAsync(CancellationToken cancellationToken)
        {
            return await _context.Clientes
                .Where(c => c.EstadoId == 2)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Estado>> ObtenerEstadosAsync(CancellationToken cancellationToken)
        {
            return await _context.Estados.ToListAsync(cancellationToken);
        }

        public async Task<int> ObtenerCantidad(CancellationToken cancellationToken)
        {
            return await _context.Clientes.CountAsync();
        }
    }
}
