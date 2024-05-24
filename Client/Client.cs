
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using Vocabulario;

namespace Client
{
    public class Client
    {
        private int numero_secuencia = 0;
        private String ruta_archivo = "Recibido.zip"; //Ruta envio archivo
        private String cadena_numero = "123456789"; //Cadena de numeros a Enviar

        private UdpClient cliente = new UdpClient(); //Crea objeto UdpClient
        Codificacion_Binaria_Client codi = new Codificacion_Binaria_Client(); //Crea objeto Codificacion_Binaria_Client
        IPEndPoint remoteIPEndPoint = new IPEndPoint(IPAddress.Any, 0); //Crea objeto IPEndPoint, punto de recepcion de ACKs
        byte[]? paquete_recivido;//Array de bytes para almacenar los datos del emisor, es opcional, null si no se recibe nada
        ACK? ack_recivido;// Almacena ACK decodificado, opcinal, null si no se recibe ningun ACK
        Stopwatch time = new Stopwatch(); //Temporizador

        private int estado = 0; //Variable que indica el estado del Cliente comienza en InitialState  
        public String _Ruta_archivo
        {
            get { return ruta_archivo; }
            set { ruta_archivo = value; }
        }
        public int _Numero_secuencia
        {
            get { return numero_secuencia; }
            set { numero_secuencia = value; }
        }
        public UdpClient _Cliente
        {
            get { return cliente; }
            set { cliente = value; }
        }
        public int _Estado
        {
            get { return estado; }
            set { estado = value; }
        }
        public Client()
        {
            Console.WriteLine("Cliente activo");
            numero_secuencia = _Numero_secuencia;
            cliente = _Cliente;
            estado = _Estado;
            ruta_archivo = _Ruta_archivo;
        }
        public void Run()
        {
            try
            {
                List<Byte> mensaje_selecionado = new List<Byte>(); //Lista que almacena en bytes los datos que se enviaran al emisor
                bool ultimo_paquete = false; //Booleano para aseguar que el paquete no se envie varias veces

                int seleccionar = _Seleccionar();  //Llama a funcion seleccionar, el usuario escoge si Enviar archivo o cadena de numeros

                mensaje_selecionado = _Almacenar(seleccionar, cadena_numero); //Llama a _Almacenar, convierte el mensaje enviado en bytes

                int indice = 0; //Variable indice para acceder a los datos
                byte paquete_actual = mensaje_selecionado[indice]; //Acceso al primer byte del mensaje       
                Datos mensaje = new Datos(paquete_actual, _Numero_secuencia); //Creamos objeto Datos con el byte y el numero de secuencia
                byte[] enviando = codi.Codificar(mensaje); //Se codifica el mensaje


                for (int aux = 0; aux < (mensaje_selecionado.Count); aux++) //Bucle para el control del envio de paquetes
                {
                    if (aux == mensaje_selecionado.Count - 1)
                    { //Comprobamos si aux es igual que el contador del mensaje -1
                        this._Numero_secuencia = -1; //En ese caso, es el ultimo paquete que se enviara
                    }
                    if (_Numero_secuencia == 0) //Este caso indica el inicio de la transmsión de paquetes
                    {
                        Enviar(enviando, "localhost", 4444); //Se envia paquete
                        _Estado = 1; //Estado del cliente indicando que espera respuesta del servidor

                        do //Bucle que se ejecutara mientras no haya recibido ACK o hayan pasado 1000 ms
                        {
                            Recibir(); //Se recibe ACK
                            Console.WriteLine("ACK RECIVIDO-> " + ack_recivido!.num_secuencia); //Se muestra numero de secuencia recibido
                            if (ack_recivido.num_secuencia != _Numero_secuencia) //Verificamos si se corresponde con el paquete actual
                            {
                                Enviar(enviando, "localhost", 4444); //Si no coincide se reenvia el paquete 
                                _Estado = 1; //estado espera de respuesta
                            }
                        } while (time.ElapsedMilliseconds >= 1000 | paquete_recivido == null);
                        time.Stop(); //Se detiene el temporizador
                        _Numero_secuencia++; //Aumenta numero de secuencia
                        indice++; //Aumenta indice, para pasar al siguiente elemento de la lista
                        _Estado = 0; //Estado 0, para Enviar el siguiente paquete
                    }
                    else
                    {
                        while (_Numero_secuencia != -1) //Bucle que dura hasta que se envie el ultimo paquete
                        {
                            bool boolean = Probabilidad(20); //Probabilidad de fallo en el envio del mensaje
                            if (boolean == true) //Si es true, se simula la perdida del paquete
                            {
                                Console.WriteLine("Error al Enviar el paquete");
                            }
                            else
                            {
                                //Al ser mayor, el paquete se envia con normalidad
                                switch (_Estado)
                                {
                                    case 0: //En caso de cliente en estado 0 (Enviar el siguiente paquete)
                                        paquete_actual = mensaje_selecionado[indice]; //Obtenemos el siguiente byte del mensaje
                                        mensaje = new Datos(paquete_actual, _Numero_secuencia); //Creamos objeto SNF_Data con el siguiente byte y su numero de secuencia
                                        enviando = codi.Codificar(mensaje); //Codoficamos el mensaje

                                        _Estado = 1; //Estado del cliente a 1, para esperar la respuesta del servidor
                                        break;

                                    case 1: //En caso de cliente en estado 1 (esperando respuesta)
                                        Enviar(enviando, "localhost", 4444); //Se envia paquete al servidor
                                        do //Espera recepcion del ACk durante 1 segundo
                                        {
                                            Recibir(); //Se recibe el paquete
                                            if (ack_recivido!.num_secuencia != _Numero_secuencia) //Si el ACK no coincide con el numero de secuencia
                                            {
                                                Enviar(enviando, "localhost", 4444); //reenviamos paquete
                                                _Estado = 1; //Estado a 1 para esperar respuesta del server
                                            }
                                        } while (time.ElapsedMilliseconds >= 1000 | paquete_recivido == null);

                                        Console.WriteLine("Mensaje con ACK {" + ack_recivido.num_secuencia + "} ha llegado al receptor satisfactoriamente"); //Mensaje de recepcion correcta
                                        _Estado = 0; //Estado a 0 para Enviar el siguiente paquete
                                        time.Stop(); //Paramos el temporizador
                                        _Numero_secuencia++; //Aumentamos el numero de secuencia esperado
                                        if (indice < mensaje_selecionado.Count - 1) //Si hay mas mensajes por Enviar
                                        {
                                            indice++; //Pasamos al siguiente mensaje de la lista
                                            if (indice >= mensaje_selecionado.Count - 1)
                                            { //Comprobamos si es el ultimo mensaje de la lista
                                                _Numero_secuencia = -1; //Indicamos que es el ultimo mensaje de la lista
                                            }
                                        }
                                        else
                                        { //Si no hay mas mensajes
                                            _Numero_secuencia = -1; //Numero de secuencia -1, indicando que es el ultimo paquete
                                        }
                                        break;
                                }
                            }
                        }
                        if (_Numero_secuencia == -1 && ultimo_paquete == false) //Verificamos que queda solo el ultimo paquete de la lista
                        {
                            ultimo_paquete = true; //Indicamos que es el ultimo paquete
                            paquete_actual = mensaje_selecionado[indice]; //Guardamos el ultimo paquete
                            mensaje = new Datos(paquete_actual, _Numero_secuencia); //Creamos objeto Data con el mensaje y el numero de secuencia
                            enviando = codi.Codificar(mensaje); //Codificamos el mensaje
                            Enviar(enviando, "localhost", 4444); //Enviamos el paquete
                            _Estado = 1; //Ponemos estado a 1 para esperar respuesta
                            do //Bucle que espera la recepcion del ACK durante 1 segundo
                            {
                                Recibir(); //Se recibe el paquete                 
                                if (ack_recivido!.num_secuencia != _Numero_secuencia)  //Si el ACK no coincide con el numero de secuencia
                                {
                                    Enviar(enviando, "localhost", 4444); //reenviamos paquete 
                                    _Estado = 1; //Estado a 1 para esperar respuesta del server
                                }
                            } while (time.ElapsedMilliseconds == 1000 | paquete_recivido == null);
                            Console.WriteLine("Mensaje con ACK " + ack_recivido.num_secuencia + " ha llegado al receptor satisfactoriamente"); //Mensaje de recepcion correcta
                            Console.WriteLine("Este era el último paquete a Enviar"); //Mensaje de aviso de ultimo mensaje
                            time.Stop(); //Paramos el temporizador

                        }
                    }
                }
                _Cliente.Close();
            }
            catch (Exception e)
            { //Capturamos excepciones
                Console.WriteLine(e.ToString());
            }
        }

        //Funcion para que el usuario escoja si Enviar cadena numerica o archivo
        static int _Seleccionar()
        {
            Console.WriteLine("Enviar cadena numérica: 1"); //Mensajes de explicacion para el usuario
            Console.WriteLine("Enviar archivo: 2");

            int seleccionar = Convert.ToInt32(Console.ReadLine()); //Leemos eleccion del usuario

            while (seleccionar != 1 && seleccionar != 2) //Bucle para que el usuario solo pueda escoger 1 o 2
            {
                Console.WriteLine("Esa opcion no existe, intentar de nuevo ");
                Console.WriteLine();
                Console.WriteLine("Enviar secuancia numérica: Introduzca 1");
                Console.WriteLine("Enviar archivo: Introduzca 2");
                seleccionar = Convert.ToInt32(Console.ReadLine());
            }
            return seleccionar; //Devuelve el valor seleccionado
        }

        //Funcion con dos argumentos: seleccion del usuario y cadena a Enviar
        List<Byte> _Almacenar(int seleccionar, string _number = "123456789")
        { 
            List<Byte> mensaje_selecionado = new List<Byte>(); //Inicializacion de lista de bytes para almacenar los datos a Enviar
            if (seleccionar == 1) //Si el usuario escogio Enviar la cadena numerica
            {
                String numero_x = _number; //Guardamos la cadena proporcionada por el usuario
                Console.WriteLine("Enviando secuencia");
                foreach (char c in numero_x) //Bucle que recorre cada caracter de la cadena numerica
                {
                    mensaje_selecionado.Add(Convert.ToByte(Convert.ToInt32(c.ToString()))); //Convertimos cada caracter de la cadena en entero, luego en byte, y se agrega a la lista
                }
            }
            else if (seleccionar == 2) //Si el usuario escogio Enviar archivo
            {
                String? ruta = null; //Ruta donde almacenar el archivo
                while (!(File.Exists(_Ruta_archivo))) //Bucle si la ruta no existe
                {
                    Console.WriteLine("Archivo no encontrado introduzca una ruta correcta: ");
                    ruta = Console.ReadLine(); //Se pide de nuevo la ruta
                }
                if (ruta != null) //Comprobamos que la ruta no el null
                {
                    _Ruta_archivo = ruta;
                }
                byte[] almacen_archivo_ruta = File.ReadAllBytes(_Ruta_archivo); //Leemos el contenido del archivo especificado en la ruta, y se almacena en almacen_archivo_ruta
                foreach (byte element in almacen_archivo_ruta) //Bucle que recorre cada elemento de almacen_archivo_ruta
                {
                    mensaje_selecionado.Add(element); //y lo agrega a la lista
                }
            }
            return mensaje_selecionado; //Se retorna la lista con el archivo
        }
        void Enviar(byte[] package, string host, int port)
        { //Funcion que recibe bytes a Enviar, cadena con IP o nombre del receptor y puerto del receptor
            _Cliente.Send(package, package.Length, host, port); //Envia el paquete al destino indicado
            time.Start(); //Inicia temporizador
        }

        void Recibir() //Funcion para recibir ACK
        {
            paquete_recivido = _Cliente.Receive(ref remoteIPEndPoint); //Recibe paquete del servidor y lo almacena
            ack_recivido = codi.Decodificar(paquete_recivido); //Decodifica el paquete recibido
        }
        static bool Probabilidad(int LostPercent)
        { //Funcion para simular la perdida de paquetes, recibe la probabilidad de perdida
            Random numero_aleatorio = new Random(); //Objeto para generar numero aleatorio
            int perdidio = numero_aleatorio.Next(100); //Generamos un numero aleatorio entre 0 y 99
            bool boolean = (perdidio < LostPercent); //Booleano que es true si el numero aleatorio es menor que la probabilidad de perdida, lo que simularia que se pierde el paquete
            return boolean;
        }
    }
}