using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Vocabulario;

namespace Vocabulario
{
    public class Codificacion_Binaria_Server : Interface_codificacion_servidor //Clase BinaryCodec de la interfaz Interface_codificacion_servidor
    {
        public byte[] Codificar(ACK mensaje) //Método Codificar, recibe objeto ACK y devuelve un array de bytes
        {
            int secuecia = mensaje.num_secuencia; //Obtener numero de secuencia del ACK

            //Declaracion de variables
            byte[] byte_Buffer; //Almacena temporalmente el mensaje codificado
            MemoryStream ms = new MemoryStream(); //Almacena temporalmente los datos que se enviaran en el flujo de datos
            BinaryWriter writer = new BinaryWriter(ms); //Escribe los datos en el flujo de datos

            writer.Write(secuecia); //Escribimos el numero de secuencia en el flujo de datos
            writer.Flush(); //Aseguramos que se han escrito todos los datos en el flujo de datos

            byte_Buffer = ms.ToArray(); //Convierte los datos del MemoryStream en un array de bytes

            return byte_Buffer; //Devuelve el array de bytes
        }
        public Datos Decodificar(byte[] byteMensaje) //Recibe array de bytes y lo convierte en ACK
        {
            MemoryStream ms = new MemoryStream(byteMensaje); //MemoryStream asociado al array de bytes recibido
            BinaryReader reader = new BinaryReader(ms); //Lee el array de bytes del memory stream

            byte mensa_recivido = reader.ReadByte(); //Lee byte del flujo de datos que corresponde al mensaje recibido

            int mensa_numero_secuencia = reader.ReadInt32(); //Lee entero de 32 bits del flujo de datos que corresponde al numero de secuencia del mensaje recibido

            Datos mensaje_recivid = new Datos(mensa_recivido, mensa_numero_secuencia); //Crea un objeto Data con el byte y el numero de secuencia

            return mensaje_recivid; //Devuelve el objeto que contiene los datos decodificados del mensaje recibido
        }
    }
}