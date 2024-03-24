using Microsoft.AspNetCore.Mvc;
using PasajeBus.Entidades;
using PasejeBus.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace PasajeBusWeb.Controllers
{
    public class AccesoController : Controller
    {
        private readonly UsuarioData _usuarioData;
        public AccesoController(UsuarioData usuarioData)
        {
            _usuarioData = usuarioData;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Correo, string Clave)
        {
            if (Correo == null || Clave == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }


            Usuario usuario = await _usuarioData.Obtener(Correo, Clave);
            if(usuario.IdUsuario == 0)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }

            ViewData["Mensaje"] = null;

            //aqui guarderemos la informacion de nuestro usuario
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Actor, $"{usuario.Nombres} {usuario.Apellidos}"),
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Role,usuario.TipoUsuario),
                new Claim(ClaimTypes.Name,usuario.Nombres),
                new Claim(ClaimTypes.Surname,usuario.Apellidos),
                new Claim(ClaimTypes.Email,usuario.Correo)
            };


            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties properties = new AuthenticationProperties()
            {
                AllowRefresh = true
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

            string rol = usuario.TipoUsuario;
            //if (rol == "Admin") return RedirectToAction("Index", "Citas");
            //if (rol == "Pasajero") return RedirectToAction("Index", "Home");

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Registrarse()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Registrarse(string Nombres, string Apellidos, string Correo, string Clave)
        {
            string resultado = await _usuarioData.Crear(new Usuario
            {
                Nombres = Nombres,
                Apellidos = Apellidos,
                Correo = Correo,
                Clave = Clave
            });
            ViewBag.Mensaje = resultado;

            if (resultado == "")
            {
                ViewBag.Creado = true;
                ViewBag.Mensaje = "Su cuenta ha sido creada.";
            }
            return View();
        }



        public IActionResult Denegado()
        {
            return View();
        }

        public async Task<IActionResult> Salir()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
