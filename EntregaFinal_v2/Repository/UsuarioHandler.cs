using Usuarios.Models;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Reflection;

namespace UsuarioHandlers
{

    public static class UsuarioHandler
    {
        public const string Connection_String = "Server=ARASALP190583\\LOCALDB;Database=SistemaGestion;Trusted_Connection=True";

        /*
        Este método va a retornar una lista de Usuarios recibiendo como parámetro el Nombre de Usuario
        Si encuentra el usuario devuelve todos sus datos, sino devuelve un objeto vacío con IDUsuario = 0
        */
        public static List<Usuario> ObtenerUsuarios(string NombreUsuario)
        {
            //Objeto usuario que es lo que el método tiene que retornar con datos o null
            Usuario obj_usuario = new Usuario();
            List<Usuario> Resultados = new List<Usuario>();

            try
            {
                using (SqlConnection sqlconnection = new SqlConnection(Connection_String))
                {
                    string QuerySelect = "SELECT * FROM Usuario WHERE NombreUsuario = @NombreUsuario";
                    
                    //Parámetros
                    SqlParameter param_NombreUsuario = new SqlParameter("NombreUsuario", System.Data.SqlDbType.VarChar) { Value = NombreUsuario };
                    
                    sqlconnection.Open();

                    using (SqlCommand sqlCommand = new SqlCommand(QuerySelect, sqlconnection))
                    {
                        sqlCommand.Parameters.Add(param_NombreUsuario);

                        using (SqlDataReader dataReader = sqlCommand.ExecuteReader())
                        {
                            if (dataReader.HasRows)
                            {
                                while (dataReader.Read())
                                {
                                    Usuario usuario = new Usuario();
                                    usuario.Id = Convert.ToInt32(dataReader["Id"]);
                                    usuario.Nombre = dataReader["Nombre"].ToString();
                                    usuario.Apellido = dataReader["Apellido"].ToString();
                                    usuario.Contraseña = dataReader["Contraseña"].ToString();
                                    usuario.NombreUsuario = dataReader["NombreUsuario"].ToString();
                                    usuario.Mail = dataReader["Mail"].ToString();
                                    Resultados.Add(usuario);
                                }
                            }
                            //Si no encuentra datos para ese nombre de usuario en la BD devuelve el objeto vacío con ID = 0
                            else
                            {
                                obj_usuario.Id = 0;
                                Resultados.Add(obj_usuario);
                            }
                        }
                        sqlconnection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return Resultados;
        }

        /*
        Este método va a crear un nuevo usuario y retorna un texto con el resultado
        Considera como campos requeridos: Nombre, Apellido y Nombre de Usuario
        Va a validar si el Nombre de Usuario ya existe con otro ID y va a cancelar en tal caso
        */
        public static string InsertarUsuario(Usuario obj_usuario)
        {
            //En este String vamos a capturar el resultado para devolver si se pudo modificar o no
            string Response = String.Empty;
            int registros_insertados = 0;
            bool error = false;
            int existe_id = 0;

            //Validar que los campos "claves" no estén vacíos
            if (String.IsNullOrEmpty(obj_usuario.Nombre) || String.IsNullOrEmpty(obj_usuario.Apellido) || String.IsNullOrEmpty(obj_usuario.NombreUsuario))
            {
                Response = "Error - Se debe informar al menos Nombre, Apellido y Nombre de Usuario.";
                error = true;
            }

            //Validar que el NombreUsuario no esté siendo usando en otro ID (se considera único el nombre de usuario)
            if (!error)
            {
                try
                {
                    using (SqlConnection sqlConnection = new SqlConnection(Connection_String))
                    {
                        string QuerySelect = "SELECT * FROM Usuario WHERE NombreUsuario = @NombreUsuario";

                        //Parámetros
                        SqlParameter param_NombreUsuario = new SqlParameter("NombreUsuario", System.Data.SqlDbType.VarChar) { Value = obj_usuario.NombreUsuario };

                        sqlConnection.Open();

                        SqlCommand SqlCmd = new SqlCommand(QuerySelect, sqlConnection);
                        SqlCmd.Parameters.Add(param_NombreUsuario);
                        SqlDataReader SqlRead = SqlCmd.ExecuteReader();

                        if (SqlRead.HasRows)
                        {
                            existe_id = 1;
                            Response = "Error - El Nombre de Usuario: '" + obj_usuario.NombreUsuario + "' ya existe con otro ID";
                        }
                        sqlConnection.Close();
                    }
                }
                catch (Exception ex)
                {
                    Response = "Error al actualizar Id de usuario: '" + obj_usuario.Id + "' - Detalle Error: " + ex.Message;
                }
            }


            if (!error && existe_id == 0)
            {
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
                            Response = "El usuario: '" + obj_usuario.NombreUsuario + "' ha sido creado.";
                        }
                        else
                        {
                            Response = "Error - El usuario '" + obj_usuario.NombreUsuario + "' no ha sido creado.";
                        }
                        sqlConnection.Close();
                    }
                }
                catch (Exception ex)
                {
                    Response = "Error - El usuario '" + obj_usuario.NombreUsuario + "' no ha sido creado. - Detalle Error: " + ex.Message;
                }
            }
            return Response;
        }

        /*
        Este método va a modificar los datos de un usuario y retorna un texto con el resultado 
        Va a validar si lo que se está modificando es el Nombre de Usuario y ya existe con otro ID, no lo va a permitir
        */
        public static string ModificarUsuario(Usuario obj_usuario)
        {
            //En este String vamos a capturar el resultado para devolver si se pudo modificar o no
            string Response = String.Empty;
            int registros_actualizados = 0;
            int existe_id = 0;

            //Validar que el NombreUsuario no esté siendo usando en otro ID (se considera único el nombre de usuario)
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(Connection_String))
                {
                    string QuerySelect = "SELECT * FROM Usuario WHERE NombreUsuario = @NombreUsuario AND Id != @IdUsuario";
                    
                    //Parámetros
                    SqlParameter parametroUsuarioId = new SqlParameter("IdUsuario", System.Data.SqlDbType.BigInt) { Value = obj_usuario.Id };
                    SqlParameter param_NombreUsuario = new SqlParameter("NombreUsuario", System.Data.SqlDbType.VarChar) { Value = obj_usuario.NombreUsuario };

                    sqlConnection.Open();

                    SqlCommand SqlCmd = new SqlCommand(QuerySelect, sqlConnection);
                    SqlCmd.Parameters.Add(parametroUsuarioId);
                    SqlCmd.Parameters.Add(param_NombreUsuario);
                    SqlDataReader SqlRead = SqlCmd.ExecuteReader();

                    if (SqlRead.HasRows)
                    {
                        existe_id = 1;
                        Response = "Error - El Nombre de Usuario: " + obj_usuario.NombreUsuario + " ya existe con otro ID";
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Response = "Error al actualizar Id de usuario: " + obj_usuario.Id + " - Detalle Error: " + ex.Message;
            }
            
            if (existe_id == 0)
            {
                try
                {
                    using (SqlConnection sqlConnection = new SqlConnection(Connection_String))
                    {
                        string QueryUpdate = "UPDATE Usuario SET Nombre = @Nombre, Apellido = @Apellido, NombreUsuario = @NombreUsuario, Contraseña = @Contraseña, Mail = @Mail WHERE Id = @IdUsuario";

                        //Parámetros
                        SqlParameter parametroUsuarioId = new SqlParameter("IdUsuario", System.Data.SqlDbType.Int) { Value = obj_usuario.Id };
                        SqlParameter param_Nombre = new SqlParameter("Nombre", System.Data.SqlDbType.VarChar) { Value = obj_usuario.Nombre };
                        SqlParameter param_Apellido = new SqlParameter("Apellido", System.Data.SqlDbType.VarChar) { Value = obj_usuario.Apellido };
                        SqlParameter param_NombreUsuario = new SqlParameter("NombreUsuario", System.Data.SqlDbType.VarChar) { Value = obj_usuario.NombreUsuario };
                        SqlParameter param_Contraseña = new SqlParameter("Contraseña", System.Data.SqlDbType.VarChar) { Value = obj_usuario.Contraseña };
                        SqlParameter param_Mail = new SqlParameter("Mail", System.Data.SqlDbType.VarChar) { Value = obj_usuario.Mail };

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
                            Response = "Error - No se ha actualizado información para el Id de usuario: " + obj_usuario.Id;
                        }
                        sqlConnection.Close();
                    }
                }
                catch (Exception ex)
                {
                    Response = "Error al actualizar Id de usuario: " + obj_usuario.Id + " - Detalle Error: " + ex.Message;
                }
            }
            return Response;
        }

        /*
        Este método va a eliminar un usuario según ID y retorna un texto con el resultado
        Primero se eliminan los datos de la tabla ProductoVendido porque tiene FK
        Después se eliminan los datos de la tabla Producto porque tiene FK
        Por último se elimina de la tabla Usuario
        */
        public static string EliminarUsuario(int IDUsuario)
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

                    //Primero se deben eliminar los datos de la tabla ProductoVendido porque tiene un FK
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
                        Response = "El usuario: '" + IDUsuario + "' ha sido eliminado.";
                    }
                    else
                    {
                        Response = "El usuario: '" + IDUsuario + "' no ha sido eliminado.";
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Response = "El usuario: '" + IDUsuario + "' no ha sido eliminado. - Detalle Error: " + ex.Message;
            }
            return Response;
        }

        /*
        Este método va a validar los datos de acceso de un usuario y devuelve un objeto 
        Recibe el nombre de usuario y contraseña, si lo encuentra devuelve todos los datos, sino un objeto vacío con ID = 0
        */
        public static Usuario ValidarAccesoUsuario(string user, string psw)
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