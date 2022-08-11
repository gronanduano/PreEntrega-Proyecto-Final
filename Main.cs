using System.Data.SqlClient;
using System.Data;
using System;

namespace EntregaFinal
{
    public class ClaseMain
    {
        static void Main(string[] args)
        {
            //Variables para controlar las opciones del Menú
            int valor;
            string opcion;
            
            do
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("************************************");
                Console.WriteLine("         PROBAR METODOS             ");
                Console.WriteLine("************************************");
                Console.WriteLine("\n" + "<A> Traer Usuario" +
                                  "\n" + "<B> Traer Producto" +
                                  "\n" + "<C> Traer Productos Vendidos" +
                                  "\n" + "<D> Traer Ventas" +
                                  "\n" + "<E> Inicio de Sesión" +
                                  "\n" + "<0> Salir");
                
                //Leer la opción ingresada y validarla
                Console.Write("\nIngrese la opción: ");
                opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "0": continue;
                    case "A": TraerUsuario(); break;
                    case "B": TraerProducto(); break;
                    case "C": TraerProductosVendidos(); break;
                    case "D": TraerVentas(); break;
                    case "E": InicioSesion(); break;
                    default: Console.WriteLine("\nOpción '" + opcion + "' no es válida\n"); break;
                }
            } while (!opcion.Contains("0"));


            //Consigna A - Traer Usuario: Recibe como parámetro un nombre del usuario, buscarlo en la base de datos y devolver el objeto con todos sus datos
            //(Esto se hará para la página en la que se mostrara los datos del usuario y en la página para modificar sus datos).
            
            void TraerUsuario()
            {
                UsuarioHandler usuario_handler = new UsuarioHandler();
                Usuario obj_usuario = new Usuario();

                obj_usuario = usuario_handler.BuscarDatosUsuarioxNombre("gronanduano");
                Console.WriteLine("Breakpoint");
            }

            //Consigna B - Traer Producto: Recibe un número de IdUsuario como parámetro, debe traer todos los productos cargados en la base de este usuario en particular.
            
            void TraerProducto()
            {
                ProductoHandler producto_handler = new ProductoHandler();
                List<Producto> listadoproductos = new List<Producto>();

                listadoproductos = producto_handler.BuscarProductosxIDUsuario(1);
                Console.WriteLine("Breakpoint");
            }

            //Consigna C - Traer Productos Vendidos: Traer Todos los productos vendidos de un Usuario, cuya información está en su producto
            //(Utilizar dentro de esta función el "Traer Productos" anteriormente hecho para saber que productosVendidos ir a buscar).
            
            void TraerProductosVendidos()
            {
                ProductoVendidoHandler productovendido_handler = new ProductoVendidoHandler();
                List<ProductoVendido> listadoproductos = new List<ProductoVendido>();

                listadoproductos = productovendido_handler.BuscarProductosVendidos(1);
                Console.WriteLine("Breakpoint");
            }

            //Consigna D - Traer Ventas: Recibe como parámetro un IdUsuario, debe traer todas las ventas de la base asignados al usuario particular.
            
            void TraerVentas()
            {
                VentaHandler venta_handler = new VentaHandler();
                List<Venta> listadoventas = new List<Venta>();

                listadoventas = venta_handler.BuscarVentasTotales(2);
                Console.WriteLine("Breakpoint");
            }

            //Consigna E - Inicio de sesión: Se le pase como parámetro el nombre del usuario y la contraseña, buscar en la base de datos si el usuario existe
            //y si coincide con la contraseña lo devuelve (el objeto Usuario), caso contrario devuelve uno vacío (Con sus datos vacíos y el id en 0).
            
            void InicioSesion()
            {
                UsuarioHandler usuario_handler = new UsuarioHandler();
                Usuario obj_usuario = new Usuario();

                obj_usuario = usuario_handler.ValidarAccesoUsuario("sjobs","mal");
                Console.WriteLine("Breakpoint");
            }
        }
    }
}