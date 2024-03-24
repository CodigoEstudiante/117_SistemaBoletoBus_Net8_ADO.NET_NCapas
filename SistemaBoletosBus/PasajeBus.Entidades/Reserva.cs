using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasajeBus.Entidades
{
    public class Reserva
    {
        public Viaje Viaje { get; set; } = null!;
        public Pasajero Pasajero { get; set; } = null!;
        public int AsientosReservados { get; set; }
        public string MontoTotal { get; set; } = null!;
        public string IdAsientos { get; set; } = null!;
    }
}
