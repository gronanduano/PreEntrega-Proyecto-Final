using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace EntregaFinal
{
    //Atributos de la Clase ProductoVendido
    public class ProductoVendido
    {
        public int IdUsuario { get; set; }
        public int IdProducto { get; set; }
        public string Descripciones { get; set; }
        public int Stock { get; set; }
        public double PrecioVenta { get; set; }
    }
}