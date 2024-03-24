using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasajeBus.Entidades
{
    public class Ruta
    {
        public int IdRuta { get; set; }
        public string Origen { get; set; } = null!;
        public string Destino { get; set; } = null!;
        public string FechaCreacion { get; set; } = null!;
    }
}
