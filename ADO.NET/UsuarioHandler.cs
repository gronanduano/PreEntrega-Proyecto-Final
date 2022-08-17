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

        //Este método va a crear un nuevo usuario y retorna un texto con el resultado 
        public string InsertarUsuario(Usuario obj_usuario)
        {
            //En este String vamos a capturar el resultado para devolver si se pudo modificar o no
            string Response = String.Empty;
            int registros_insertados = 0;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(Connection_String))
                {
                    string QueryUpdate = "INSERT INTO Usuario ( Nombre, Apellido, NombreUsuario, Contraseña, Mail ) VALUES " +
                        "( @Nombre, @Apellido, @NombreUsuario, @Contraseña, @Mail )";

                    //Parámetros
                    SqlParameter param_Nombre = new SqlParameter("Nombre", SqlDbType.VarChar) { Value = obj_usuario.Nombre };
                    SqlParameter param_Apellido = new SqlParameter("Apellido", SqlDbType.VarChar) { Value = obj_usuario.Apellido };
                    SqlParameter param_NombreUsuario = new SqlParameter("NombreUsuario", SqlDbType.VarChar) { Value = obj_usuario.NombreUsuario };
                    SqlParameter param_Contraseña = new SqlParameter("Contraseña", SqlDbType.VarChar) { Value = obj_usuario.Contraseña };
                    SqlParameter param_Mail = new SqlParameter("Mail", SqlDbType.VarChar) { Value = obj_usuario.Mail };

                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(QueryUpdate, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(param_Nombre);
                        sqlCommand.Parameters.Add(param_Apellido);
                        sqlCommand.Parameters.Add(param_NombreUsuario);
                        sqlCommand.Parameters.Add(param_Contraseña);
                        sqlCommand.Parameters.Add(param_Mail);
                        registros_insertados = sqlCommand.ExecuteNonQuery();
                    }
                    if (registros_insertados == 1)
                    {
                        Response = "El usuario ha sido creado.";
                    }
                    else
                    {
                        Response = "El usuario no ha sido creado.";
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Response = "El usuario no ha sido creado. - Detalle Error: " + ex.Message;
            }
            return Response;
        }

        //Este método va a modificar los datos de un usuario y retorna un texto con el resultado 
        public string ModificarUsuario(Usuario obj_usuario)
        {
            //En este String vamos a capturar el resultado para devolver si se pudo modificar o no
            string Response = String.Empty;
            int registros_actualizados = 0;
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(Connection_String))
                {
                    string QueryUpdate = "UPDATE Usuario SET Nombre = @Nombre, Apellido = @Apellido, NombreUsuario = @NombreUsuario, Contraseña = @Contraseña, Mail = @Mail WHERE Id = @IdUsuario";

                    //Parámetros IdUsuario
                    SqlParameter parametroUsuarioId = new SqlParameter("IdUsuario", System.Data.SqlDbType.BigInt) { Value = obj_usuario.Id };
                    SqlParameter param_Nombre = new SqlParameter("Nombre", System.Data.SqlDbType.BigInt) { Value = obj_usuario.Nombre };
                    SqlParameter param_Apellido = new SqlParameter("Apellido", System.Data.SqlDbType.BigInt) { Value = obj_usuario.Apellido };
                    SqlParameter param_NombreUsuario = new SqlParameter("NombreUsuario", System.Data.SqlDbType.BigInt) { Value = obj_usuario.NombreUsuario };
                    SqlParameter param_Contraseña = new SqlParameter("Contraseña", System.Data.SqlDbType.BigInt) { Value = obj_usuario.Contraseña };
                    SqlParameter param_Mail = new SqlParameter("Mail", System.Data.SqlDbType.BigInt) { Value = obj_usuario.Mail };

                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(QueryUpdate, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(parametroUsuarioId);
                        sqlCommand.Parameters.Add(param_Nombre);
                        sqlCommand.Parameters.Add(param_Apellido);
                        sqlCommand.Parameters.Add(param_NombreUsuario);
                        sqlCommand.Parameters.Add(param_Contraseña);
                        sqlCommand.Parameters.Add(param_Mail);
                        registros_actualizados = sqlCommand.ExecuteNonQuery();
                    }
                    if (registros_actualizados == 1)
                    {
                        Response = "Se ha actualizado el Id de usuario: " + obj_usuario.Id;
                    }
                    else
                    {
                        Response = "No se ha actualizado información para el Id de usuario: " + obj_usuario.Id;
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Response = "Error al actualizar Id de usuario: " + obj_usuario.Id + " - Detalle Error: " + ex.Message;
            }
            return Response;
        }

        //Este método va a eliminar un usuario según ID y retorna un texto con el resultado 
        public string EliminarUsuario(int IDUsuario)
        {
            //En este String vamos a capturar el resultado para devolver si se pudo modificar o no
            string Response = String.Empty;
            int registros_eliminados = 0;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(Connection_String))
                {
                    //Parámetros
                    SqlParameter param_IdUsuario = new SqlParameter("IdUsuario", System.Data.SqlDbType.BigInt) { Value = IDUsuario };

                    //Primero se deben eliminar los datos de la tabla producto_vendido porque tiene un FK
                    string QueryDelete_prodvendido = "DELETE pv FROM ProductoVendido pv JOIN Producto p ON pv.IdProducto = p.Id JOIN Usuario u ON p.IdUsuario = u.Id WHERE u.Id = @IdUsuario";

                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(QueryDelete_prodvendido, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(param_IdUsuario);
                        registros_eliminados = sqlCommand.ExecuteNonQuery();
                        sqlCommand.Parameters.Clear();
                    }
                    sqlConnection.Close();

                    //Después se deben eliminar los datos de la tabla producto porque tiene un FK
                    string QueryDelete_producto = "DELETE p FROM Producto p JOIN Usuario u ON p.IdUsuario = u.Id WHERE u.Id = @IdUsuario";
                    
                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(QueryDelete_producto, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(param_IdUsuario);
                        registros_eliminados = sqlCommand.ExecuteNonQuery();
                        sqlCommand.Parameters.Clear();
                    }
                    sqlConnection.Close();

                    //Después se elimina de la tabla Usuario
                    string QueryUpdate = "DELETE FROM Usuario WHERE Id = @IdUsuario";

                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(QueryUpdate, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(param_IdUsuario);
                        registros_eliminados = sqlCommand.ExecuteNonQuery();
                    }
                    if (registros_eliminados == 1)
                    {
                        Response = "El usuario ha sido eliminado.";
                    }
                    else
                    {
                        Response = "El usuario no ha sido eliminado.";
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Response = "El usuario no ha sido eliminado. - Detalle Error: " + ex.Message;
            }
            return Response;
        }
    }
}
