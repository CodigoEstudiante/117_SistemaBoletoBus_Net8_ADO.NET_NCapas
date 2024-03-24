
namespace PasajeBus.Entidades
{
    public class Viaje
    {
        public int IdViaje { get; set; }
        public Bus Bus { get; set; } = null!;
        public Ruta Ruta { get; set; } = null!;
        public string FechaSalida { get; set; } = null!;
        public string HoraSalida { get; set; } = null!;
        public string FechaLlegada { get; set; } = null!;
        public string HoraLlegada { get; set; } = null!;
        public string Precio { get; set; } = null!;
        public int TotalAsientos { get; set; }
        public int AsientosReservados { get; set; }
        public int AsientoDisponibles { get; set; }
        public int Completo { get; set; }
        public string FechaCreacion { get; set; } = null!;
    }
}
