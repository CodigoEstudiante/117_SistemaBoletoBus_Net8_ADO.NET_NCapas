using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasajeBus.Entidades.DTO
{
    public class ViajeUsuario
    {
        public int IdReserva { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public string NombreBus { get; set; }
        public string FechaSalida { get; set; }
        public string HoraSalida { get; set; }
        public string FechaLlegada { get; set; }
        public string HoraLlegada { get; set; }
        public string Precio { get; set; }
        public int CantidadAsientos { get; set; }
        public string MontoTotal { get; set; }
        public string NumeroAsientos { get; set; }
    }
}
