using Ventas.Models;
using System.Data.SqlClient;
using System.Data;
using Ventas.DTOS;

namespace VentaHandlers
{
    public static class VentaHandler
    {
        public const string Connection_String = "Server=ARASALP190583\\LOCALDB;Database=SistemaGestion;Trusted_Connection=True";

        //Este método va a calcular el total de ventas según un IDUsuario y los devuelve en una tabla
        public static List<Venta> BuscarVentasTotales(int IDUsuario)
        {
            //Tabla temporal para almacenarar el resultado de la consulta
            DataTable temp_table = new DataTable();

            List<Venta> Lista_Ventas = new List<Venta>();

            using (SqlConnection sqlConnection = new SqlConnection(Connection_String))
            {
                using (SqlCommand sqlCommand = new SqlCommand("select p.IdUsuario, pv.IdProducto, p.Descripciones, pv.Stock, p.PrecioVenta " +
                    "from ProductoVendido pv join Producto p on pv.IdProducto = p.Id " +
                    "where p.IdUsuario = @IDUsuario", sqlConnection))
                {
                    sqlConnection.Open();

                    sqlCommand.Parameters.AddWithValue("@IDUsuario", IDUsuario);
                    SqlDataAdapter SqlAdapter = new SqlDataAdapter();
                    SqlAdapter.SelectCommand = sqlCommand;
                    SqlAdapter.Fill(temp_table);

                    foreach (DataRow line in temp_table.Rows)
                    {
                        Venta obj_venta = new Venta();
                        obj_venta.IdUsuario = Convert.ToInt32(line["IdUsuario"]);
                        obj_venta.IdProducto = Convert.ToInt32(line["IdProducto"]);
                        obj_venta.Descripciones = line["Descripciones"].ToString();
                        obj_venta.Stock = Convert.ToInt32(line["Stock"]);
                        obj_venta.PrecioVenta = Convert.ToDouble(line["PrecioVenta"]);
                        obj_venta.Valor_Venta = obj_venta.Stock * obj_venta.PrecioVenta;
                        Lista_Ventas.Add(obj_venta);
                    }
                    sqlConnection.Close();
                }
            }
            return Lista_Ventas;
        }

        /*
        Este método va a crear una venta según IDProducto, IDUsuario y Cantidad Vendida
        1. Primero se va a validar que el Producto exista
        2. Después se valida que haya stock disponible de ese Producto para vender
        3. Después se valida que el Usuario exista
        3. Se registran las ventas en las tablas: A) Venta B) ProductoVendido y por último C) Se actualiza Producto con el nuevo stock
        4. El método retorna una tabla con los datos de la venta y una columna con el status del registro
        */
        public static List<PostVenta> InsertarVentas(List<PostVenta> DetalleVenta)
        {
            //Buscamos todos los ID de Producto para no hacer un select por cada item de venta
            DataTable dtProductos = new DataTable();
            DataTable dtUsuarios = new DataTable();
            DataRow[] singlequery;
            DataTable dtIdVenta = new DataTable();
            string query = string.Empty;
            int registros_insertados = 0;
            int stock_producto = 0;
            int cont = -1;

            //Buscamos todos los ID de Producto y Stock para no hacer un select por cada item de venta
            using (SqlConnection sqlConnection = new SqlConnection(Connection_String))
            {
                SqlDataAdapter SqlAdapter = new SqlDataAdapter("select Id, Stock from Producto", sqlConnection);
                sqlConnection.Open();
                SqlAdapter.Fill(dtProductos);
                sqlConnection.Close();
            }
            //Buscamos todos los ID de Usuario para no hacer un select por cada item de venta
            using (SqlConnection sqlConnection = new SqlConnection(Connection_String))
            {
                SqlDataAdapter SqlAdapter = new SqlDataAdapter("select Id from Usuario", sqlConnection);
                sqlConnection.Open();
                SqlAdapter.Fill(dtUsuarios);
                sqlConnection.Close();
            }
            //Se recorren los datos de venta recibidos por la API
            foreach (var line in DetalleVenta)
            {
                cont++;

                //Validar que el ID de Producto exista y que haya stock suficiente para registrar la venta
                query = "Id = " + line.IdProducto.ToString();
                singlequery = dtProductos.Select(query);
                
                if (singlequery.Length == 0)
                {
                    DetalleVenta[cont].Status = "Venta no Registrada - No existe el producto";
                    continue;
                }
                else
                {
                    if (line.Stock > Convert.ToInt32(singlequery[0].ItemArray[1]))
                    {
                        DetalleVenta[cont].Status = "Venta no Registrada - No hay Stock suficiente del producto";
                        continue;
                    }
                    else
                    {
                        stock_producto = Convert.ToInt32(singlequery[0].ItemArray[1]) - line.Stock;
                    }
                }

                //Validar que el ID de Usuario exista
                query = "Id = " + line.IdUsuario.ToString();
                singlequery = dtUsuarios.Select(query);

                if (singlequery.Length == 0)
                {
                    DetalleVenta[cont].Status = "Venta no Registrada - No existe el Usuario";
                    continue;
                }

                //Insertar en la tabla Venta (Id automatico y Comentarios: DateTime + IdUsuario vendedor)
                try
                {
                    using (SqlConnection sqlConnection = new SqlConnection(Connection_String))
                    {
                        string QueryUpdate = "INSERT INTO Venta ( Comentarios ) VALUES ( @Comentarios )";

                        //Parámetros
                        SqlParameter param_Comentarios = new SqlParameter("Comentarios", SqlDbType.VarChar) { Value = "Venta Registrada: " + DateTime.Now };

                        sqlConnection.Open();
                        using (SqlCommand sqlCommand = new SqlCommand(QueryUpdate, sqlConnection))
                        {
                            sqlCommand.Parameters.Add(param_Comentarios);
                            registros_insertados = sqlCommand.ExecuteNonQuery();
                        }
                        if (registros_insertados == 1)
                        {
                            //Obtener IDVenta generado
                            using (SqlConnection sqlConnection_id = new SqlConnection(Connection_String))
                            {
                                SqlDataAdapter SqlAdapter = new SqlDataAdapter("select max(Id) from Venta", sqlConnection);
                                SqlAdapter.Fill(dtIdVenta);
                            }

                            DetalleVenta[cont].Status = "Venta Registrada - Id Venta: " + dtIdVenta.Rows[0].ItemArray[0] + " - IdUsuario: " + line.IdUsuario;
                        }
                        else
                        {
                            DetalleVenta[cont].Status = "Venta No Registrada - Error al ingresar venta";
                        }
                        sqlConnection.Close();
                    }
                }
                catch (Exception ex)
                {
                    DetalleVenta[cont].Status = "Venta No Registrada - Error al ingresar venta: " + ex.Message;
                }

                //Insertar en la tabla Producto vendido
                try
                {
                    using (SqlConnection sqlConnection = new SqlConnection(Connection_String))
                    {
                        string QueryInsert = "INSERT INTO ProductoVendido ( Stock, IdProducto, IdVenta ) VALUES ( @Stock, @IdProducto, @IdVenta )";

                        //Parámetros
                        SqlParameter param_Stock = new SqlParameter("Stock", SqlDbType.Int) { Value = line.Stock };
                        SqlParameter param_IdProducto = new SqlParameter("IdProducto", SqlDbType.Int) { Value = line.IdProducto };
                        SqlParameter param_IdVenta = new SqlParameter("IdVenta", SqlDbType.Int) { Value = dtIdVenta.Rows[0].ItemArray[0] };

                        sqlConnection.Open();
                        using (SqlCommand sqlCommand = new SqlCommand(QueryInsert, sqlConnection))
                        {
                            sqlCommand.Parameters.Add(param_Stock);
                            sqlCommand.Parameters.Add(param_IdProducto);
                            sqlCommand.Parameters.Add(param_IdVenta);
                            registros_insertados = sqlCommand.ExecuteNonQuery();
                            sqlCommand.Parameters.Clear();
                        }
                        if (registros_insertados == 1)
                        {
                            DetalleVenta[cont].Status = "Venta Registrada - Id Venta: " + dtIdVenta.Rows[0].ItemArray[0] + " - IdUsuario: " + line.IdUsuario;
                        }
                        else
                        {
                            DetalleVenta[cont].Status = "Venta No Registrada - Error al ingresar venta";
                        }
                        sqlConnection.Close();
                    }
                }
                catch (Exception ex)
                {
                    DetalleVenta[cont].Status = "Venta No Registrada - Error al ingresar venta: " + ex.Message;
                }

                //Modificar la tabla producto (Descontar stock))
                try
                {
                    using (SqlConnection sqlConnection = new SqlConnection(Connection_String))
                    {
                        string QueryUpdate = "UPDATE Producto SET Stock = " + stock_producto + " WHERE Id = @IdProducto";

                        //Parámetros
                        SqlParameter param_IdProducto = new SqlParameter("IdProducto", SqlDbType.Int) { Value = line.IdProducto };

                        sqlConnection.Open();
                        using (SqlCommand sqlCommand = new SqlCommand(QueryUpdate, sqlConnection))
                        {
                            sqlCommand.Parameters.Add(param_IdProducto);
                            registros_insertados = sqlCommand.ExecuteNonQuery();
                            sqlCommand.Parameters.Clear();
                        }
                        if (registros_insertados == 1)
                        {
                            DetalleVenta[cont].Status = "Venta Registrada - Id Venta: " + dtIdVenta.Rows[0].ItemArray[0] + " - IdUsuario: " + line.IdUsuario;
                        }
                        else
                        {
                            DetalleVenta[cont].Status = "Venta No Registrada - Error al ingresar venta";
                        }
                        sqlConnection.Close();
                    }
                }
                catch (Exception ex)
                {
                    DetalleVenta[cont].Status = "Venta No Registrada - Error al ingresar venta: " + ex.Message;
                }
            }
            return DetalleVenta;
        }

        /*
        Este método va a eliminar una Venta según ID y retorna un texto con el resultado 
        1. Antes de eliminar de ProductoVendido capturar el stock de la venta
        2. liminar de ProductoVendido
        3. Actualizar el stock de la tabla producto
        4. Después se debe eliminar de Venta
        */
        public static string EliminarVenta(int IDVenta)
        {
            //En este String vamos a capturar el resultado para devolver si se pudo modificar o no
            string Response = String.Empty;
            int registros_eliminados = 0;
            //Tabla temporal para almacenarar el resultado de la consulta
            DataTable temp_table = new DataTable();
            int stock_vendido = 0;
            int producto_vendido = 0;

            try
            {
                using (SqlConnection sqlConnection = new SqlConnection(Connection_String))
                {
                    //Parámetros
                    SqlParameter param_IDVenta = new SqlParameter("IDVenta", System.Data.SqlDbType.BigInt) { Value = IDVenta };

                    //Obtener la cantidad vendida antes de eliminar de ProductoVendido
                    string QueryStockVendido = "SELECT IdProducto, Stock from ProductoVendido WHERE IDVenta = @IDVenta";
                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(QueryStockVendido, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@IDVenta", IDVenta);
                        SqlDataAdapter SqlAdapter = new SqlDataAdapter();
                        SqlAdapter.SelectCommand = sqlCommand;
                        SqlAdapter.Fill(temp_table);

                        if (temp_table.Rows.Count > 0)
                        {
                            foreach (DataRow line in temp_table.Rows)
                            {
                                stock_vendido = Convert.ToInt32(line["Stock"]);
                                producto_vendido = Convert.ToInt32(line["IdProducto"]);
                                //Considero que va a haber un único registro
                                break;
                            }
                        }
                    }
                    sqlConnection.Close();

                    //Primero se deben eliminar los datos de la tabla producto_vendido porque tiene un FK
                    string QueryDelete_prodvendido = "DELETE FROM ProductoVendido WHERE IDVenta = @IDVenta";

                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(QueryDelete_prodvendido, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(param_IDVenta);
                        registros_eliminados = sqlCommand.ExecuteNonQuery();
                        sqlCommand.Parameters.Clear();
                    }
                    sqlConnection.Close();

                    //Modificar la tabla producto (Ajustar stock))
                    string QueryUpdate_stock = "UPDATE Producto SET stock = ( stock + " + stock_vendido + ") where Id = @IDProducto";
                    SqlParameter param_IDProducto = new SqlParameter("IDProducto", System.Data.SqlDbType.Int) { Value = producto_vendido };

                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(QueryUpdate_stock, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(param_IDProducto);
                        registros_eliminados = sqlCommand.ExecuteNonQuery();
                    }
                    if (registros_eliminados == 1)
                    {
                        Response = "El ID de venta: '" + IDVenta + "' se ha eliminado correctamente.";
                    }
                    else
                    {
                        Response = "El ID de venta: '" + IDVenta + "' no se ha podido eliminar.";
                    }
                    sqlConnection.Close();


                    //Después se elimina de la tabla Venta
                    string QueryDeleteVenta = "DELETE FROM Venta WHERE Id = @IDVenta";

                    sqlConnection.Open();
                    using (SqlCommand sqlCommand = new SqlCommand(QueryDeleteVenta, sqlConnection))
                    {
                        sqlCommand.Parameters.Add(param_IDVenta);
                        registros_eliminados = sqlCommand.ExecuteNonQuery();
                    }
                    if (registros_eliminados == 1)
                    {
                        Response = "El ID de venta: '" + IDVenta + "' se ha eliminado correctamente.";
                    }
                    else
                    {
                        Response = "El ID de venta: '" + IDVenta + "' no se ha podido eliminar.";
                    }
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                Response = "El ID de venta: '" + IDVenta + "' no se ha podido eliminar. Detalle Error: " + ex.Message;
            }
            return Response;
        }
    }
}
