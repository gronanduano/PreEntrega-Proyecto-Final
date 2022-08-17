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
                                  "\n" + "<F> Modificar Usuario" +
                                  "\n" + "<G> Crear Usuario" +
                                  "\n" + "<H> Eliminar Usuario" +
                                  "\n" + "<I> Crear Producto" +
                                  "\n" + "<J> Modificar Producto" +
                                  "\n" + "<K> Eliminar Producto" +
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
                    case "F": ModificarUsuario(); break;
                    case "G": CrearUsuario(); break;
                    case "H": EliminarUsuario(); break;
                    case "I": CrearProducto(); break;
                    case "J": ModificarProducto(); break;
                    case "K": EliminarProducto(); break;
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

                listadoventas = venta_handler.BuscarVentasTotales(1);
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

            //Desafío Entregable Clase 15

            void ModificarUsuario()
            {
                UsuarioHandler usuario_handler = new UsuarioHandler();
                Usuario obj_usuario = new Usuario();
                string respuesta = string.Empty;

                obj_usuario.Id = 99;
                obj_usuario.Nombre = "Juan José";
                obj_usuario.Apellido = "Bottero";
                obj_usuario.NombreUsuario = "jbotter";
                obj_usuario.Contraseña = "esa";
                obj_usuario.Mail = "jbotter@hotmail.com";

                respuesta = usuario_handler.ModificarUsuario(obj_usuario);
                Console.WriteLine("Breakpoint");
            }

            void CrearUsuario()
            {
                UsuarioHandler usuario_handler = new UsuarioHandler();
                Usuario obj_usuario = new Usuario();
                string respuesta = string.Empty;

                obj_usuario.Nombre = "Emilio";
                obj_usuario.Apellido = "Tomson";
                obj_usuario.NombreUsuario = "etomson";
                obj_usuario.Contraseña = "12345";
                obj_usuario.Mail = "etomson@hotmail.com";

                respuesta = usuario_handler.InsertarUsuario(obj_usuario);
                Console.WriteLine("Breakpoint");
            }

            void EliminarUsuario()
            {
                UsuarioHandler usuario_handler = new UsuarioHandler();
                string respuesta = string.Empty;
                int ID_Usuario_eliminar = 2;

                respuesta = usuario_handler.EliminarUsuario(ID_Usuario_eliminar);
                Console.WriteLine("Breakpoint");
            }

            void CrearProducto()
            {
                ProductoHandler producto_handler = new ProductoHandler();
                Producto obj_producto = new Producto();
                string respuesta = string.Empty;

                obj_producto.Descripciones = "Bufanda";
                obj_producto.Costo = 120.35;
                obj_producto.PrecioVenta = 140.55;
                obj_producto.Stock = 19;
                obj_producto.IdUsuario = 2;

                respuesta = producto_handler.InsertarProducto(obj_producto);
                Console.WriteLine("Breakpoint");
            }

            void ModificarProducto()
            {
                ProductoHandler producto_handler = new ProductoHandler();
                Producto obj_producto = new Producto();
                string respuesta = string.Empty;

                obj_producto.Id = 14;
                obj_producto.Descripciones = "Bufanda Larga";
                obj_producto.Costo = 120.36;
                obj_producto.PrecioVenta = 140.56;
                obj_producto.Stock = 21;
                obj_producto.IdUsuario = 99;

                respuesta = producto_handler.ModificarProducto(obj_producto);
                Console.WriteLine("Breakpoint");
            }

            void EliminarProducto()
            {
                ProductoHandler producto_handler = new ProductoHandler();
                int ID_Producto_eliminar = 10;
                string respuesta = string.Empty;

                respuesta = producto_handler.EliminarProducto(ID_Producto_eliminar);
                Console.WriteLine("Breakpoint");
            }
        }
    }
}