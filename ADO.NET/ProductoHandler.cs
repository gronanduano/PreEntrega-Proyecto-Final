using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace EntregaFinal
{
    public class ProductoHandler : DbHandler
    {
        // CONSIGNA <B> Traer Producto

        //Este método va a buscar los productos en la BD según el IDUsuario y lo devuelve en una tabla
        public List<Producto> BuscarProductosxIDUsuario(int IDUsuario)
        {
            //Tabla temporal para almacenar el resultado de la consulta
            DataTable temp_table = new DataTable();

            List<Producto> Lista_productos = new List<Producto>();
            Producto obj_producto = new Producto();

            using (SqlConnection sqlConnection = new SqlConnection(Connection_String))
            {
                using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Producto WHERE IdUsuario = @IDUsuario", sqlConnection))
                {
                    sqlConnection.Open();

                    sqlCommand.Parameters.AddWithValue("@IDUsuario", IDUsuario);
                    SqlDataAdapter SqlAdapter = new SqlDataAdapter();
                    SqlAdapter.SelectCommand = sqlCommand;
                    SqlAdapter.Fill(temp_table);

                    foreach (DataRow line in temp_table.Rows)
                    {
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
    }
}
