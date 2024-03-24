using Microsoft.Extensions.Options;
using PasajeBus.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasejeBus.Data
{
    public class ReservaData
    {
        private readonly ConnectionStrings con;
        public ReservaData(IOptions<ConnectionStrings> options)
        {
            con = options.Value;
        }
        public async Task<string> Reserva(Reserva objeto)
        {

            string respuesta = "";
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {

                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_guardarReserva", conexion);
                cmd.Parameters.AddWithValue("@IdViaje", objeto.Viaje.IdViaje);
                cmd.Parameters.AddWithValue("@IdPasajero", objeto.Pasajero.IdPasajero);
                cmd.Parameters.AddWithValue("@AsientosReservados", objeto.AsientosReservados);
                cmd.Parameters.AddWithValue("@MontoTotal", objeto.MontoTotal);
                cmd.Parameters.AddWithValue("@IdAsientos", objeto.IdAsientos);
                cmd.Parameters.Add("@msgError", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                    respuesta = Convert.ToString(cmd.Parameters["@msgError"].Value)!;
                }
                catch
                {
                    respuesta = "Error al procesar";
                }
            }
            return respuesta;
        }
    }
}
