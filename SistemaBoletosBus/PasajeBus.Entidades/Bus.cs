
namespace PasajeBus.Entidades
{
    public class Bus
    {
        public int IdBus { get; set; }
        public string NumeroPlaca { get; set; } = null!;
        public string Nombre { get; set; } = null!;
        public int CapacidadPiso1 { get; set; }
        public int CapacidadPiso2 { get; set; }
        public int CapacidadTotal { get; set; }
        public bool Disponible { get; set; }
        public string FechaCreacion { get; set; } = null!;
    }
}
