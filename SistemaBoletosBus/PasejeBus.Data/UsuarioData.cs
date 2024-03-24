using Microsoft.Extensions.Options;
using PasajeBus.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using PasajeBus.Entidades.DTO;

namespace PasejeBus.Data
{
    public class UsuarioData
    {
        private readonly ConnectionStrings con;
        public UsuarioData(IOptions<ConnectionStrings> options)
        {
            con = options.Value;
        }

        public async Task<Usuario> Obtener(string correo, string clave)
        {
            Usuario usuario = new Usuario();

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_obtenerUsuario", conexion);
                cmd.Parameters.AddWithValue("@Correo", correo);
                cmd.Parameters.AddWithValue("@Clave", clave);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    if (await dr.ReadAsync())
                    {
                        usuario = new Usuario()
                        {
                            IdUsuario = Convert.ToInt32(dr["IdUsuario"]),
                            Nombres = dr["Nombres"].ToString()!,
                            Apellidos = dr["Apellidos"].ToString()!,
                            Correo = dr["Correo"].ToString()!,
                            TipoUsuario = dr["TipoUsuario"].ToString()!,
                        };
                    }
                }
            }
            return usuario;
        }

        public async Task<string> Crear(Usuario objeto)
        {

            string respuesta = "";
            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_crearUsuario", conexion);
                cmd.Parameters.AddWithValue("@Nombres", objeto.Nombres);
                cmd.Parameters.AddWithValue("@Apellidos", objeto.Apellidos);
                cmd.Parameters.AddWithValue("@Correo", objeto.Correo);
                cmd.Parameters.AddWithValue("@Clave", objeto.Clave);
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

        public async Task<List<ViajeUsuario>> Viajes(int idUsuario)
        {
            List<ViajeUsuario> lista = new List<ViajeUsuario>();

            using (var conexion = new SqlConnection(con.CadenaSQL))
            {
                await conexion.OpenAsync();
                SqlCommand cmd = new SqlCommand("sp_obtenerViajesUsuario", conexion);
                cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var dr = await cmd.ExecuteReaderAsync())
                {
                    while (await dr.ReadAsync())
                    {
                        lista.Add(new ViajeUsuario()
                        {
                            IdReserva = Convert.ToInt32(dr["IdReserva"]),
                            Origen = dr["Origen"].ToString()!,
                            Destino = dr["Destino"].ToString()!,
                            NombreBus = dr["NombreBus"].ToString()!,
                            FechaSalida = dr["FechaSalida"].ToString()!,
                            HoraSalida = dr["HoraSalida"].ToString()!,
                            FechaLlegada = dr["FechaLlegada"].ToString()!,
                            HoraLlegada = dr["HoraLlegada"].ToString()!,
                            Precio = dr["Precio"].ToString()!,
                            CantidadAsientos = Convert.ToInt32(dr["CantidadAsientos"]),
                            MontoTotal = dr["MontoTotal"].ToString()!,
                            NumeroAsientos = dr["NumeroAsientos"].ToString()!
                        });
                    }
                }
            }
            return lista;
        }

    }
}
