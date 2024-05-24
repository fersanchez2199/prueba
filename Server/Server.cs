using System.Net;
using System.Net.Sockets;
using System.Security;
using Vocabulario;

namespace Server
{
    class Server
    {
        private UdpClient cliente = new UdpClient(4444); //Instancia de UdpCliente que escuchara en el puerto 4444

        Codificacion_Binaria_Server codec_Server = new Codificacion_Binaria_Server(); //Instacia para codificar y decodificar los mensajes

        IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0); //Instacia que indica direccion desde donde se reciben datos (todas las interfaces y puerto aleatorio)

        public UdpClient Client
        {
            get { return cliente; }
            set { cliente = value; }
        }
        public void Run() //Funcionalidad del servidor
        {
            List<Byte> numeros = new List<Byte>(); //Lista para almacenar los bytes recibidos
            int ACK_actual = -1; //Numero de secuencia del ACK actual, inicialmente -1
            int estado_servidor = 0; //Estado del servidor, inicialmente Recibir

            for (; ; )   //Bucle para ejecutar el servidor infinitamente
            {
                Datos recibido = Recibir(); //Recibe un mensaje

                if (recibido.num_secuencia == -1)
                { //Si el mensaje tiene el numero de secuencia -1,
                    estado_servidor = 1; //estado recreating
                }
                if (recibido.num_secuencia == ACK_actual + 1 && estado_servidor == 0)
                { //Si server en Recibir, y el numero de secuencia es el esperado,
                    numeros.Add(recibido.mensaje); //se añade el mensaje a la lista 
                    ACK_actual++; //y se aumenta el ACK
                }
                else if (estado_servidor == 1) //Si server en recreating,
                {
                    numeros.Add(recibido.mensaje); //se agrega el mensaje a la lista
                    Escribir_archivo(numeros.ToArray()); //y se escribe el archivo
                }
                Envia_ACK(recibido.num_secuencia); //Se envia ACK de confirmacion de recepcion

            }
        }

        void Escribir_archivo(byte[] form) //Funcion que recibe array de bytes y lo escribe en un archivo
        {
            File.WriteAllBytes("prueba.zip", form);
            Console.WriteLine("Se ha creado el archivo prueba.zip , ignorar si se ha escogido la opcion 1");
        }
        Datos Recibir() //Funcion para recibir datos del cliente y decodificarlos
        {
            byte[] byte_Buffer = Client.Receive(ref remoteIPEndPoint); //recibe los datos del cliente
            Datos recibido = codec_Server.Decodificar(byte_Buffer); //decodifica 
            Console.WriteLine("Mensaje recibido-> " + recibido.mensaje + " numero de secuencia-> " + recibido.num_secuencia); //e imprime por pantalla el mensaje y su numero de secuencia

            return recibido; //Retorna el mensaje decodificado
        }
        void Envia_ACK(int numero_secuencia) //Funcion que recibe un numero de secuencia
        {
            ACK respuesta = new ACK(numero_secuencia); //Crea objeto ACK con ese numero de secuencia
            byte[] envia = codec_Server.Codificar(respuesta); //lo codifica
            Client.Send(envia, envia.Length, remoteIPEndPoint); //y lo envia al cliente

        }
    }
}