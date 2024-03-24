using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasajeBus.Entidades;
using PasejeBus.Data;

namespace PasajeBusWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RutaController : Controller
    {
        private readonly RutaData _rutaData;
        public RutaController(RutaData rutaData)
        {
            _rutaData = rutaData;
        }
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<Ruta> lista = await _rutaData.Lista();
            return StatusCode(StatusCodes.Status200OK, new { data = lista });
        }


        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Ruta objeto)
        {
            string respuesta = await _rutaData.Crear(objeto);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] Ruta objeto)
        {
            string respuesta = await _rutaData.Editar(objeto);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int Id)
        {
            string respuesta = await _rutaData.Eliminar(Id);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }

    }
}
