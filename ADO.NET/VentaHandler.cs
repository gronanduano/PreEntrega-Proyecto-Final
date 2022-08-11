using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace EntregaFinal
{
    public class VentaHandler : DbHandler
    {
        // CONSIGNA <D> Traer Ventas

        //Este método va a calcular el total de ventas según un IDUsuario y los devuelve en una tabla
        public List<Venta> BuscarVentasTotales(int IDUsuario)
        {
            //Tabla temporal para almacenarar el resultado de la consulta
            DataTable temp_table = new DataTable();

            List<Venta> Lista_Ventas = new List<Venta>();
            Venta obj_venta = new Venta();

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
    }
}