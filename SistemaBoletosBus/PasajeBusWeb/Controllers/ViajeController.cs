using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasajeBus.Entidades;
using PasajeBus.Entidades.DTO;
using PasejeBus.Data;
using System.Security.Claims;

namespace PasajeBusWeb.Controllers
{
    
    public class ViajeController : Controller
    {
        private readonly ViajeData _viajeData;
        private readonly UsuarioData _usuarioData;
        public ViajeController(ViajeData viajeData, UsuarioData usuarioData)
        {
            _viajeData = viajeData;
            _usuarioData = usuarioData;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Pasajero")]
        public IActionResult MisViajes()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<Viaje> lista = await _viajeData.Lista();
            return StatusCode(StatusCodes.Status200OK, new { data = lista });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Viaje objeto)
        {
            string respuesta = await _viajeData.Crear(objeto);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] Viaje objeto)
        {
            string respuesta = await _viajeData.Editar(objeto);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete]
        public async Task<IActionResult> Eliminar(int Id)
        {
            string respuesta = await _viajeData.Eliminar(Id);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ObtenerAsientosReserva(int IdViaje)
        {
            List<AsientoReserva> lista = await _viajeData.ObtenerAsientosReserva(IdViaje);
            return StatusCode(StatusCodes.Status200OK, new { data = lista });
        }


        [Authorize(Roles = "Pasajero")]
        [HttpGet]
        public async Task<IActionResult> ViajesUsuario()
        {
            ClaimsPrincipal claimuser = HttpContext.User;
            string idUsuario = claimuser.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault()!;

            List<ViajeUsuario> lista = await _usuarioData.Viajes(int.Parse(idUsuario));
            return StatusCode(StatusCodes.Status200OK, new { data = lista });
        }


    }
}
