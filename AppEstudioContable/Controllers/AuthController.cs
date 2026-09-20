
using Microsoft.AspNetCore.Mvc;


using System.Diagnostics;
using ServiciosEC.Utilidades;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using ServiciosEC.Managers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authorization;
using ServiciosEC.Interfaces.Managers;

namespace AppEstudioContable.Controllers
{

    // las rutas solo tienen el nombre del método, ejemplo: localhost:5000/Login
    [Route("/{action}")]

    public class AuthController : Controller
    {
        private readonly IUsuariosManager _usuarioManager;
        public AuthController(IUsuariosManager usuarioManager)
        {
            _usuarioManager = usuarioManager;
        }


        [AllowAnonymous]
        //muestra la vista login
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        /// <summary>
        /// Recibe el nombre de usuario o email y la contraseña
        /// obtiene el usuario de la base de datos
        /// autentica al usuario
        /// </summary>
        /// <param name="collection">Formulario con usuario y contraseña</param>
        /// <returns>Redirecciona a Home si es correcto, o sino muestra la vista Login con errores</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(IFormCollection collection, CancellationToken cancellation)
        {
            try
            {
                //datos del formulario
                string email = collection["Email"];
                string contrasenia = collection["Contrasenia"];

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(contrasenia))
                {
                    throw new Exception("El email y la contraseña son obligatorios.");
                }

                //comprobar si el usuario existe
                var usuario = RegexUtilities.EsEmailValido(email) ?
                    await _usuarioManager.ObtenerPorEmailAsync(email, cancellation) :
                    await _usuarioManager.ObtenerPorNombreAsync(email, cancellation);

                if (usuario == null)
                    throw new Exception("Datos incorrectos.");


                //comprobar si la contraseña es correcta
                if (!_usuarioManager.ValidarContraseña(usuario, contrasenia))
                    throw new Exception("Datos incorrectos.");


                // lógica para autenticar al usuario
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(
                        new ClaimsIdentity(
                            [
                                new Claim(ClaimTypes.NameIdentifier, usuario.IdPersona.ToString()),
                                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                                new Claim(ClaimTypes.Email, usuario.Email),
                                new Claim(ClaimTypes.Role, usuario.Rol.Descripcion),
                            ], CookieAuthenticationDefaults.AuthenticationScheme)
                        )
                    );

                // redirigir a la página de inicio
                return RedirectToAction("Index", "Home");

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                ModelState.AddModelError("", ex.Message);
                return View();
            }

        }

        // cierra la sesion del usuario
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet("/generar-hash/{password}")]
        [AllowAnonymous]
        public IActionResult GenerarHash(string password)
        {
            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<ServiciosEC.Models.Usuario>();
            var hash = hasher.HashPassword(new ServiciosEC.Models.Usuario(), password);
            return Ok(new { password, hash });
        }
    }

}
