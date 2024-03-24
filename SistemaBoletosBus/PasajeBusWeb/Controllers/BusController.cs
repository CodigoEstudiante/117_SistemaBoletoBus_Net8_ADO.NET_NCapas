using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PasajeBus.Entidades;
using PasejeBus.Data;
using System.Numerics;

namespace PasajeBusWeb.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BusController : Controller
    {
        private readonly BusData _busData;
        public BusController(BusData busData) { 
            _busData = busData;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<Bus> lista = await _busData.Lista();
            return StatusCode(StatusCodes.Status200OK, new { data = lista });
        }


        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Bus objeto)
        {
            string respuesta = await _busData.Crear(objeto);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] Bus objeto)
        {
            string respuesta = await _busData.Editar(objeto);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int Id)
        {
            string respuesta = await _busData.Eliminar(Id);
            return StatusCode(StatusCodes.Status200OK, new { data = respuesta });
        }

    }
}
