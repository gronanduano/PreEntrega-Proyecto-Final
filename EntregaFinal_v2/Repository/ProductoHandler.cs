using Productos.Models;
using Productos.DTOS;
using System.Data.SqlClient;
using System.Data;

namespace ProductoHandlers
{

    public static class ProductoHandler
    {
        public const string Connection_String = "Server=ARASALP190583\\LOCALDB;Database=SistemaGestion;Trusted_Connection=True";

        //Este método va a retornar una lista de Productos (No tiene filtros, devuelve todos los productos registrados)
        public static List<Producto> ObtenerProductos()
        {
            //Tabla temporal para almacenar el resultado de la consulta
            DataTable temp_table = new DataTable();

            List<Producto> Lista_productos = new List<Producto>();

            using (SqlConnection sqlConnection = new SqlConnection(Connection_String))
            {
                using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Producto", sqlConnection))
                {
                    sqlConnection.Open();
                    
                    SqlDataAdapter SqlAdapter = new SqlDataAdapter();
                    SqlAdapter.SelectCommand = sqlCommand;
                    SqlAdapter.Fill(temp_table);

                    foreach (DataRow line in temp_table.Rows)
                    {
                        Producto obj_producto = new Producto();
                        obj_producto.Id = Convert.ToInt32(line["Id"]);
                        obj_producto.Descripciones = line["Descripciones"].ToString();
                        obj_producto.Costo = Convert.ToDouble(line["Costo"]);
                        obj_producto.PrecioVenta = Convert.ToDouble(line["PrecioVenta"]);
                        obj_producto.Stock = Convert.ToInt32(line["Stock"]);
                        obj_producto.IdUsuario = Convert.ToInt32(line["IdUsuario"]);
                        Lista_productos.Add(obj_producto);
                    }
                    sqlConnection.Close();
                }
            }
            return Lista_productos;
        }

        //Este método va a crear un nuevo producto y retorna un texto con el resultado. Valida que el IDUsuario exista antes de crear
        public static List<PostProducto> InsertarProductos(List<PostProducto> DetalleProducto)
        {
            DataTable dtUsuarios = new DataTable();
            DataRow[] singlequery;
            string query = string.Empty;
            int registros_insertados = 0;
            int cont = -1;

            //Buscamos todos los ID de Usuario para no hacer un select por cada item de venta
            using (SqlConnection sqlConnection = new SqlConnection(Connection_String))
            {
                SqlDataAdapter SqlAdapter = new SqlDataAdapter("select Id from Usuario", sqlConnection);
                sqlConnection.Open();
                SqlAdapter.Fill(dtUsuarios);
                sqlConnection.Close();
            }

            //Se recorren los datos de Productos recibidos por la API
            foreach (var line in DetalleProducto)
            {
                cont++;

                //Validar que el ID de Usuario exista
                query = "Id = " + line.IdUsuario.ToString();
                singlequery = dtUsuarios.Select(query);

                if (singlequery.Length == 0)
                {
                    DetalleProducto[cont].Status = "Producto: '" + line.Descripciones + "' no se ha registrado. No existe el ID de Usuario";
                    continue;
                }

                //Insertar en la tabla Producto
                try
                {
                    using (SqlConnection sqlConnection = new SqlConnection(Connection_String))
                    {
                        string QueryUpdate = "INSERT INTO Producto (Descripciones, Costo, PrecioVenta, Stock, IdUsuario) VALUES (@Descripciones, @Costo, @PrecioVenta, @Stock, @IdUsuario)";

                        //Parámetros
                        SqlParameter param_Descripciones = new SqlParameter("Descripciones", SqlDbType.VarChar) { Value = line.Descripciones };
                        SqlParameter param_Costo = new SqlParameter("Costo", SqlDbType.Decimal) { Value = line.Costo };
                        SqlParameter param_PrecioVenta = new SqlParameter("PrecioVenta", SqlDbType.Decimal) { Value = line.PrecioVenta };
                        SqlParameter param_Stock = new SqlParameter("Stock", SqlDbType.Int) { Value = line.Stock };
                        SqlParameter param_IdUsuario = new SqlParameter("IdUsuario", SqlDbType.Int) { Value = line.IdUsuario };

                        sqlConnection.Open();
                        using (SqlCommand sqlCommand = new SqlCommand(QueryUpdate, sqlConnection))
                        {
                            sqlCommand.Parameters.Add(param_Descripciones);
                            sqlCommand.Parameters.Add(param_Costo);
                            sqlCommand.Parameters.Add(param_PrecioVenta);
                            sqlCommand.Parameters.Add(param_Stock);
                            sqlCommand.Parameters.Add(param_IdUsuario);
                            registros_insertados = sqlCommand.ExecuteNonQuery();
                            sqlCommand.Parameters.Clear();
                        }
                        if (registros_insertados == 1)
                        {
                            DetalleProducto[cont].Status = "Producto: '" + line.Descripciones + "' registrado correctamente.";
                        }
                        else
                        {
                            DetalleProducto[cont].Status = "Producto: '" + line.Descripciones + "' no se ha registrado.";
                        }
                        sqlConnection.Close();
                    }
                }
                catch (Exception ex)
                {
                    DetalleProducto[cont].Status = "Producto: '" + line.Descripciones + "' no se ha registrado. Error:" + ex.Message;
                }
            }
            return DetalleProducto;
        }

        //Este método va a modificar los datos de un producto y retorna un texto con el resultado 
        public static string ModificarProducto(Producto obj_producto)
        {
            //En este String vamos a capturar el resultado para devolver si se pudo modificar o no
            string Response = String.Empty;
            int registros_actualizados = 0;
            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(Connection_String))
                {
                    string QueryUpdate = "UPDATE Producto SET Descripciones = @Descripciones, Costo = @Costo, PrecioVenta = @PrecioVenta, Stock = @Stock, IdUsuario = @IdUsuario WHERE Id = @IdProducto";

                    //Parámetros
                    SqlParameter parametroIdProducto = new SqlParameter("IdProducto", System.Data.SqlDbType.BigInt) { Value = obj_producto.Id };
                    SqlParameter param_Descripciones = new SqlParameter("Descripciones", SqlDbType.VarChar) { Value = obj_producto.Descripciones };
                    SqlParameter param_Costo = new SqlParameter("Costo", System.Data.SqlDbType.Decimal) { Value = obj_producto.Costo };
                    SqlParameter param_PrecioVenta = new SqlParameter("PrecioVenta", System.Data.SqlDbType.Decimal) { Value = obj_producto.PrecioVenta };
                    SqlParameter param_Stock = new SqlParameter("Stock", System.Data.SqlDbType.Int) { Value = obj_producto.Stock };
                    SqlParameter param_IdUsuario = new SqlParameter("IdUsuario", System.Data.SqlDbType.BigInt) { Value = obj_producto.IdUsuario };

                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(QueryUpdate, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(parametroIdProducto);
                        sqlCommand.Parameters.Add(param_Descripciones);
                        sqlCommand.Parameters.Add(param_Costo);
                        sqlCommand.Parameters.Add(param_PrecioVenta);
                        sqlCommand.Parameters.Add(param_Stock);
                        sqlCommand.Parameters.Add(param_IdUsuario);
                        registros_actualizados = sqlCommand.ExecuteNonQuery();
                    }
                    if (registros_actualizados == 1)
                    {
                        Response = "Se ha actualizado el Id de Producto: " + obj_producto.Id;
                    }
                    else
                    {
                        Response = "No se ha actualizado información para el Id de Producto: " + obj_producto.Id;
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Response = "Error al actualizar Id de Producto: " + obj_producto.Id + " - Detalle Error: " + ex.Message;
            }
            return Response;
        }

        //Este método va a eliminar un producto según ID y retorna un texto con el resultado. Primero se debe eliminar de ProductoVendido por FK 
        public static string EliminarProducto(int IDProducto)
        {
            //En este String vamos a capturar el resultado para devolver si se pudo modificar o no
            string Response = String.Empty;
            int registros_eliminados = 0;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(Connection_String))
                {
                    //Parámetros
                    SqlParameter param_IdProducto = new SqlParameter("IdProducto", System.Data.SqlDbType.BigInt) { Value = IDProducto };

                    //Primero se deben eliminar los datos de la tabla producto_vendido porque tiene un FK
                    string QueryDelete_prodvendido = "delete pv from ProductoVendido pv join Producto p on pv.IdProducto = p.Id where pv.IdProducto = @IdProducto";

                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(QueryDelete_prodvendido, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(param_IdProducto);
                        registros_eliminados = sqlCommand.ExecuteNonQuery();
                        sqlCommand.Parameters.Clear();
                    }
                    sqlConnection.Close();

                    //Después se elimina de la tabla producto
                    string QueryDeleteProducto = "DELETE FROM Producto WHERE Id = @IdProducto";

                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(QueryDeleteProducto, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(param_IdProducto);
                        registros_eliminados = sqlCommand.ExecuteNonQuery();
                    }
                    if (registros_eliminados == 1)
                    {
                        Response = "El producto: '" + IDProducto + "' ha sido eliminado.";
                    }
                    else
                    {
                        Response = "El producto: '" + IDProducto + "' no ha sido eliminado.";
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Response = "El producto: '" + IDProducto + "' no ha sido eliminado - Detalle Error: " + ex.Message;
            }
            return Response;
        }

    }
}