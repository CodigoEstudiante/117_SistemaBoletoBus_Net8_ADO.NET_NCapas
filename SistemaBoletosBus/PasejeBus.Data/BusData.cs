using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using System.Data;
using PasajeBus.Entidades;

namespace PasejeBus.Data
{
    public class BusData
    {
        private readonly ConnectionStrings con;
        public BusData(IOptions<ConnectionStrings> options)
        {
            con = options.Value;
        }

        public async Task<List<Bus>> Lista()
        {
            List<Bus> lista = new List<Bus>();

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_listaBus", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Bus()
                        {
                            IdBus = Convert.ToInt32(dr["IdBus"]),
                            NumeroPlaca = dr["NumeroPlaca"].ToString()!,
                            Nombre = dr["Nombre"].ToString()!,
                            CapacidadPiso1 = Convert.ToInt32( dr["CapacidadPiso1"].ToString()!),
                            CapacidadPiso2 = Convert.ToInt32(dr["CapacidadPiso2"].ToString()!),
                            CapacidadTotal = Convert.ToInt32(dr["CapacidadTotal"].ToString()!),
                            Disponible = Convert.ToBoolean(dr["Disponible"]!),
                            FechaCreacion = dr["FechaCreacion"].ToString()!
                        });
                    }
                }
            }
            return lista;
        }

        public async Task<string> Crear(Bus objeto)
        {

            string respuesta = "";
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_crearBus", conexion);
                cmd.Parameters.AddWithValue("@NumeroPlaca", objeto.NumeroPlaca);
                cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre);
                cmd.Parameters.AddWithValue("@CapacidadPiso1", objeto.CapacidadPiso1);
                cmd.Parameters.AddWithValue("@CapacidadPiso2", objeto.CapacidadPiso2);
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

        public async Task<string> Editar(Bus objeto)
        {

            string respuesta = "";
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_editarBus", conexion);
                cmd.Parameters.AddWithValue("@IdBus", objeto.IdBus);
                cmd.Parameters.AddWithValue("@NumeroPlaca", objeto.NumeroPlaca);
                cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre);
                cmd.Parameters.AddWithValue("@CapacidadPiso1", objeto.CapacidadPiso1);
                cmd.Parameters.AddWithValue("@CapacidadPiso2", objeto.CapacidadPiso2);
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
                SqlCommand cmd = new SqlCommand("sp_eliminarBus", conexion);
                cmd.Parameters.AddWithValue("@IdBus", Id);
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
