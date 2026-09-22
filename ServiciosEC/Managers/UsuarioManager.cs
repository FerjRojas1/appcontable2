using Microsoft.AspNetCore.Identity;
using PasswordVerificationResult = Microsoft.AspNetCore.Identity.PasswordVerificationResult;
using ServiciosEC.Models;
using Microsoft.EntityFrameworkCore;
using ServiciosEC.Models.DTOs;
using System.Linq;
using ServiciosEC.Interfaces.Managers;

namespace ServiciosEC.Managers
{
    /// <summary>
    /// <inheritdoc cref="IManager{Usuario}"/>
    /// </summary>
    public class UsuarioManager : IUsuariosManager
    {

        private readonly ECContext _context;
        public UsuarioManager(ECContext context)
        {
            _context = context;
        }

        public async Task<Usuario?> ObtenerPorNombreAsync(string nombre, CancellationToken cancellationToken)
        {
            return await _context
                .Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.NombreUsuario == nombre, cancellationToken);
        }


        public async Task<Usuario?> ObtenerPorEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _context
                .Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task EditarSinContrasenia(Usuario usuario, CancellationToken cancellationToken)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync(cancellationToken);
        }


        public bool ValidarContraseña(Usuario u, string contraseniaIngresada)
        {
            var hasher = new PasswordHasher<Usuario>();
            var resultado = hasher.VerifyHashedPassword(u, u.Contrasenia, contraseniaIngresada);
            return resultado == PasswordVerificationResult.Success;
        }


        // Modificación: El tipo de retorno ahora es DatosExistenciaUsuario
        public async Task<DatosExistenciaUsuario> DatosExistentesAsync(Usuario usuario, CancellationToken cancellationToken)
        {
            var resultado = new DatosExistenciaUsuario { Existe = false, Campo = null, IdExistente = null };

            var usuarioExistenteEmail = await _context.Usuarios
                .Where(u => u.IdPersona != usuario.IdPersona && u.Email == usuario.Email)
                .FirstOrDefaultAsync(cancellationToken);

            if (usuarioExistenteEmail != null)
            {
                resultado.Existe = true;
                resultado.Campo = "Email";
                resultado.IdExistente = usuarioExistenteEmail.IdPersona;
                return resultado;
            }

            var usuarioExistenteNombreUsuario = await _context.Usuarios
                .Where(u => u.IdPersona != usuario.IdPersona && u.NombreUsuario == usuario.NombreUsuario)
                .FirstOrDefaultAsync(cancellationToken);

            if (usuarioExistenteNombreUsuario != null)
            {
                resultado.Existe = true;
                resultado.Campo = "Nombre de Usuario";
                resultado.IdExistente = usuarioExistenteNombreUsuario.IdPersona;
                return resultado;
            }

            if (!string.IsNullOrWhiteSpace(usuario.Dni))
            {
                var usuarioExistenteDni = await _context.Usuarios
                    .Where(u => u.IdPersona != usuario.IdPersona && u.Dni == usuario.Dni)
                    .FirstOrDefaultAsync(cancellationToken);

                if (usuarioExistenteDni != null)
                {
                    resultado.Existe = true;
                    resultado.Campo = "DNI";
                    resultado.IdExistente = usuarioExistenteDni.IdPersona;
                    return resultado;
                }
            }

            return resultado;
        }


        public async Task<List<Role>> ObtenerRoleUsuarioAsync(CancellationToken cancellationToken)
        {
            return await _context.Roles
                .Where(r => r.IdRol != (int)ECContext.RolesEnum.Cliente)
                .ToListAsync(cancellationToken);
        }

        public async Task Insertar(Usuario usuario, CancellationToken cancellationToken)
        {
            var hasher = new PasswordHasher<Usuario>();
            usuario.Contrasenia = hasher.HashPassword(new Usuario(), usuario.Contrasenia);


            //si existe un usuario con los mismos datos, pero esta inactivo...
            var eliminado = _context.Usuarios.IgnoreQueryFilters()
                .FirstOrDefault(u =>
                            (u.NombreUsuario == usuario.NombreUsuario || u.Email == usuario.Email)
                            && u.EstadoId == (int)ECContext.EstadosEnum.Inactivo);

            if (eliminado != null)
            {
                // ... lo vuelvo a activar
                eliminado.EstadoId = (int)ECContext.EstadosEnum.Activo;

                // reemplazo los datos por los nuevos
                eliminado.NombreUsuario = usuario.NombreUsuario;
                eliminado.Email = usuario.Email;
                eliminado.Telefono = usuario.Telefono;
                eliminado.Contrasenia = usuario.Contrasenia;
                eliminado.RolId = usuario.RolId;
                eliminado.IdPersona = usuario.IdPersona; // Esto puede causar un problema si IdPersona se genera automáticamente

                _context.Update(eliminado);

            }
            else
            {
                // Usuario nuevo: agregar y forzar discriminador
                _context.Usuarios.Add(usuario);
                _context.Entry(usuario).Property("Discriminator").CurrentValue = "Usuario";
            }

            

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Usuario>> ObtenerTodos(CancellationToken cancellationToken)
        {
            return await _context
                .Usuarios
                .Include(u => u.Rol)
                .Where(u => u.EstadoId == 1)
                .ToListAsync(cancellationToken);
        }

        public async Task<Usuario> ObtenerPorId(int id, CancellationToken cancellationToken)
        {
            var usuario = await _context
                .Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.IdPersona == id, cancellationToken);

            if (usuario is null)
                throw new KeyNotFoundException($"No se encontró el usuario con ID {id}");

            return usuario;

        }

        public async Task Editar(Usuario usuario, CancellationToken cancellationToken)
        {
            var hasher = new PasswordHasher<Usuario>();
            usuario.Contrasenia = hasher.HashPassword(new Usuario(), usuario.Contrasenia);

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task Borrar(int id, CancellationToken cancellationToken)
        {
            var usuario = await ObtenerPorId(id, cancellationToken);

            if (usuario is not null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync(cancellationToken);
            }
        }


        public async Task<IEnumerable<Usuario>> Buscar(string filtroBusqueda, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(filtroBusqueda))
            {
                //return await ObtenerTodos(cancellationToken);
                return await _context
                    .Usuarios
                    
                    //.Where(u => u.EstadoId == 1) //innecesario, siempre deberia traer los activos
                    .ToListAsync(cancellationToken);
            }

            filtroBusqueda = filtroBusqueda.ToLower();

            var lista = await _context.Usuarios
                
                .Where(u => u.Nombre.ToLower().Contains(filtroBusqueda)
                    || u.Apellido.ToLower().Contains(filtroBusqueda)
                    || u.NombreUsuario.ToLower().Contains(filtroBusqueda)
                    || u.Email.ToLower().Contains(filtroBusqueda)
                    || u.Dni.ToLower().Contains(filtroBusqueda))
                .ToListAsync(cancellationToken);

            return lista;
        }

        public Task<int> ObtenerCantidad(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Usuario>> ObtenerInactivosAsync(CancellationToken cancellationToken)
        {
            return await _context.Usuarios
                .Include(c => c.Rol)
                .Where(c => c.EstadoId == 2)
                .ToListAsync(cancellationToken);
        }


        public async Task<IEnumerable<Auditoria>> MostrarActividad(CancellationToken cancellationToken)
        {
            return await _context.Auditorias
                .Include(a => a.Persona)
                .ToListAsync();
        }
    }
}