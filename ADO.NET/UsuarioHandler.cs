using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace EntregaFinal
{
    public class UsuarioHandler : DbHandler
    {
        // CONSIGNA <A> Traer Usuario

        //Este método va a buscar un usuario en la BD y los devuelve en un objeto 
        public Usuario BuscarDatosUsuarioxNombre(string username)
        {
            //Tabla temporal para almacenarar el resultado de la consulta
            DataTable temp_table = new DataTable();

            //Objeto usuario que es lo que el método tiene que retornar con datos o null
            Usuario obj_usuario = new Usuario();

            using (SqlConnection sqlConnection = new SqlConnection(Connection_String))
            {
                using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Usuario WHERE NombreUsuario = @username", sqlConnection))
                {
                    sqlConnection.Open();

                    sqlCommand.Parameters.AddWithValue("@username", username);
                    SqlDataAdapter SqlAdapter = new SqlDataAdapter();
                    SqlAdapter.SelectCommand = sqlCommand;
                    SqlAdapter.Fill(temp_table);

                    foreach (DataRow line in temp_table.Rows)
                    {
                        obj_usuario.Id = Convert.ToInt32(line["Id"]);
                        obj_usuario.Nombre = line["Nombre"].ToString();
                        obj_usuario.Apellido = line["Apellido"].ToString();
                        obj_usuario.Contraseña = line["Contraseña"].ToString();
                        obj_usuario.NombreUsuario = line["NombreUsuario"].ToString();
                        obj_usuario.Mail = line["Mail"].ToString();
                        
                        //Considero que va a haber un único NombreUsuario así que devuelvo el 1ero que encuentro
                        break;
                    }
                    sqlConnection.Close();
                }
            }
            return obj_usuario;
        }

        // CONSIGNA <E> Inicio de sesión

        //Este método va a validar los datos de acceso de un usuario y devuelve un objeto 
        public Usuario ValidarAccesoUsuario(string user, string psw)
        {
            //Tabla temporal para almacenarar el resultado de la consulta
            DataTable temp_table = new DataTable();

            //Objeto usuario que es lo que el método tiene que retornar con datos o null
            Usuario obj_usuario = new Usuario();

            using (SqlConnection sqlConnection = new SqlConnection(Connection_String))
            {
                using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Usuario WHERE NombreUsuario = @user and Contraseña = @psw", sqlConnection))
                {
                    sqlConnection.Open();

                    sqlCommand.Parameters.AddWithValue("@user", user);
                    sqlCommand.Parameters.AddWithValue("@psw", psw);
                    SqlDataAdapter SqlAdapter = new SqlDataAdapter();
                    SqlAdapter.SelectCommand = sqlCommand;
                    SqlAdapter.Fill(temp_table);

                    if (temp_table.Rows.Count > 0)
                    {
                        foreach (DataRow line in temp_table.Rows)
                        {
                            obj_usuario.Id = Convert.ToInt32(line["Id"]);
                            obj_usuario.Nombre = line["Nombre"].ToString();
                            obj_usuario.Apellido = line["Apellido"].ToString();
                            obj_usuario.Mail = line["Mail"].ToString();
                            obj_usuario.NombreUsuario = user;
                            obj_usuario.Contraseña = psw;

                            //Considero que va a haber un único usuario con esos datos así que devuelvo el 1ero que encuentro
                            break;
                        }
                    }
                    else
                    {
                        //Si no encuentra datos para ese usuario y contraseña en la BD devuelve el objeto vacío con ID = 0
                        obj_usuario.Id = 0;
                    }
                    sqlConnection.Close();
                }
            }
            return obj_usuario;
        }
    }
}
