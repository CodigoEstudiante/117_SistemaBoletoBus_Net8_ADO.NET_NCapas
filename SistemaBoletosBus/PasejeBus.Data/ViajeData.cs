using Microsoft.Extensions.Options;
using PasajeBus.Entidades;
using System.Data.SqlClient;
using System.Data;
using PasajeBus.Entidades.DTO;

namespace PasejeBus.Data
{
    public class ViajeData
    {
        private readonly ConnectionStrings con;
        public ViajeData(IOptions<ConnectionStrings> options)
        {
            con = options.Value;
        }
        public async Task<List<Viaje>> Lista()
        {
            List<Viaje> lista = new List<Viaje>();

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_listaViaje", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Viaje()
                        {
                            IdViaje = Convert.ToInt32(dr["IdViaje"]),
                            Bus = new Bus
                            {
                                IdBus = Convert.ToInt32(dr["IdBus"]),
                                NumeroPlaca = dr["NumeroPlaca"].ToString()!,
                                Nombre = dr["Nombre"].ToString()!
                            },
                            Ruta = new Ruta
                            {
                                IdRuta = Convert.ToInt32(dr["IdRuta"]),
                                Origen = dr["Origen"].ToString()!,
                                Destino = dr["Destino"].ToString()!
                            },
                            FechaSalida = dr["FechaSalida"].ToString()!,
                            HoraSalida = dr["HoraSalida"].ToString()!,
                            FechaLlegada = dr["FechaLlegada"].ToString()!,
                            HoraLlegada = dr["HoraLlegada"].ToString()!,
                            Precio = dr["Precio"].ToString()!,
                            TotalAsientos = Convert.ToInt32(dr["TotalAsientos"]!),
                            AsientosReservados = Convert.ToInt32(dr["AsientosReservados"]!),
                            AsientoDisponibles = Convert.ToInt32(dr["AsientoDisponibles"]!),
                            Completo = Convert.ToInt32(dr["Completo"]!),
                            FechaCreacion = dr["FechaCreacion"].ToString()!
                        });
                    }
                }
            }
            return lista;
        }
        public async Task<string> Crear(Viaje objeto)
        {

            string respuesta = "";
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {

                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_crearViaje", conexion);
                cmd.Parameters.AddWithValue("@IdBus", objeto.Bus.IdBus);
                cmd.Parameters.AddWithValue("@IdRuta", objeto.Ruta.IdRuta);
                cmd.Parameters.AddWithValue("@FechaSalida", objeto.FechaSalida);
                cmd.Parameters.AddWithValue("@HoraSalida", objeto.HoraSalida);
                cmd.Parameters.AddWithValue("@FechaLlegada", objeto.FechaLlegada);
                cmd.Parameters.AddWithValue("@HoraLlegada", objeto.HoraLlegada);
                cmd.Parameters.AddWithValue("@Precio", objeto.Precio);
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

        public async Task<string> Editar(Viaje objeto)
        {

            string respuesta = "";
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {

                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_editarViaje", conexion);
                cmd.Parameters.AddWithValue("@IdViaje", objeto.IdViaje);
                cmd.Parameters.AddWithValue("@IdBus", objeto.Bus.IdBus);
                cmd.Parameters.AddWithValue("@IdRuta", objeto.Ruta.IdRuta);
                cmd.Parameters.AddWithValue("@FechaSalida", objeto.FechaSalida);
                cmd.Parameters.AddWithValue("@HoraSalida", objeto.HoraSalida);
                cmd.Parameters.AddWithValue("@FechaLlegada", objeto.FechaLlegada);
                cmd.Parameters.AddWithValue("@HoraLlegada", objeto.HoraLlegada);
                cmd.Parameters.AddWithValue("@Precio", objeto.Precio);
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
        public async Task<string> Eliminar(int Id)
        {

            string respuesta = "";
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_eliminarViaje", conexion);
                cmd.Parameters.AddWithValue("@IdViaje", Id);
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

        public async Task<List<AsientoReserva>> ObtenerAsientosReserva(int idViaje)
        {
            List<AsientoReserva> lista = new List<AsientoReserva>();

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_obtenerAsientosReserva", conexion);
                cmd.Parameters.AddWithValue("@IdViaje", idViaje);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new AsientoReserva()
                        {
                            IdAsiento = Convert.ToInt32(dr["IdAsiento"]),
                            IdBus = Convert.ToInt32(dr["IdBus"]),
                            NumeroPiso = Convert.ToInt32(dr["NumeroPiso"]),
                            NumeroAsiento = Convert.ToInt32(dr["NumeroAsiento"]),
                            IdReserva = Convert.ToInt32(dr["IdReserva"])
                        });
                    }
                }
            }
            return lista;
        }


    }
}
