using Microsoft.AspNetCore.Mvc;
using PasajeBus.Entidades;
using PasajeBusWeb.Models;
using PasejeBus.Data;
using System.Diagnostics;
using System.Security.Claims;

namespace PasajeBusWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ReservaData _reservaData;
        private readonly UsuarioData _usuarioData;
     
        public HomeController(ILogger<HomeController> logger, ReservaData reservaData, UsuarioData usuarioData)
        {
            _logger = logger;
            _reservaData = reservaData;
            _usuarioData = usuarioData;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ObtenerUsuario()
        {
            Usuario usuario;
            ClaimsPrincipal claimuser = HttpContext.User;

            if (claimuser.Identity!.IsAuthenticated)
            {
                usuario = new Usuario()
                {
                    IdUsuario = Convert.ToInt32(claimuser.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault()!),
                    Nombres = claimuser.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault()!,
                    Apellidos = claimuser.Claims.Where(c => c.Type == ClaimTypes.Surname).Select(c => c.Value).SingleOrDefault()!,
                    Correo = claimuser.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault()!,
                };
            }
            else
                usuario = new Usuario();



            return StatusCode(StatusCodes.Status200OK, new { data = usuario });
        }

        [HttpPost]
        public async Task<IActionResult> Reservar([FromBody] Reserva objeto)
        {
            string respuesta = await _reservaData.Reserva(objeto);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
