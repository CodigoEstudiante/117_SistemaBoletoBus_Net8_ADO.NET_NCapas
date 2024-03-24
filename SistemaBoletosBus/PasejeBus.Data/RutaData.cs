

using Microsoft.Extensions.Options;
using PasajeBus.Entidades;
using System.Data.SqlClient;
using System.Data;

namespace PasejeBus.Data
{
    public class RutaData
    {
        private readonly ConnectionStrings con;
        public RutaData(IOptions<ConnectionStrings> options)
        {
            con = options.Value;
        }

        public async Task<List<Ruta>> Lista()
        {
            List<Ruta> lista = new List<Ruta>();

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_listaRutas", conexion);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new Ruta()
                        {
                            IdRuta = Convert.ToInt32(dr["IdRuta"]),
                            Origen = dr["Origen"].ToString()!,
                            Destino = dr["Destino"].ToString()!,
                            FechaCreacion = dr["FechaCreacion"].ToString()!
                        });
                    }
                }
            }
            return lista;
        }

        public async Task<string> Crear(Ruta objeto)
        {

            string respuesta = "";
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_crearRuta", conexion);
                cmd.Parameters.AddWithValue("@Origen", objeto.Origen);
                cmd.Parameters.AddWithValue("@Destino", objeto.Destino);
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

        public async Task<string> Editar(Ruta objeto)
        {

            string respuesta = "";
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_editarRuta", conexion);
                cmd.Parameters.AddWithValue("@IdRuta", objeto.IdRuta);
                cmd.Parameters.AddWithValue("@Origen", objeto.Origen);
                cmd.Parameters.AddWithValue("@Destino", objeto.Destino);
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
                SqlCommand cmd = new SqlCommand("sp_eliminarRuta", conexion);
                cmd.Parameters.AddWithValue("@IdRuta", Id);
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
