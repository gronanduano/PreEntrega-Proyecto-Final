using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace EntregaFinal
{
    //Atributos de la Clase Venta
    public class Venta
    {
        public int IdUsuario { get; set; }
        public int IdProducto { get; set; }
        public string Descripciones { get; set; }
        public int Stock { get; set; }
        public double PrecioVenta { get; set; }
        public double Valor_Venta { get; set; }
    }
}
