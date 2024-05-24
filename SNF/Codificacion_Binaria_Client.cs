using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Vocabulario;

namespace Vocabulario
{
    public class Codificacion_Binaria_Client : Interface_codificacion_client
    {
        public byte[] Codificar(Datos mensaje) //Toma un objeto Data como parametro y devuelve un array de bytes
        {
            int secuecia = mensaje.num_secuencia; //Obtiene numero de secuencia del mensaje

            byte mensa = mensaje.mensaje; //Obtiene mensaje que se va a codificar

            byte[] byte_Buffer; //Almacenar el mensaje codificado temporalmente
            MemoryStream ms = new MemoryStream(); //Almacena temporalmente los datos que se enviaran en el flujo de datos
            BinaryWriter writer = new BinaryWriter(ms); //Escribe los datos en el flujo de datos

            writer.Write(mensa); //Escribimos el mensaje en el flujo de datos
            writer.Write(secuecia); //Escribimos el numero de secuencia en el flujo de datos
            writer.Flush(); //Aseguramos que se han escrito todos los datos en el flujo de datos

            byte_Buffer = ms.ToArray(); //Convertimos el flujo de datos en un array de bytes (contiene el mensaje codificado)

            return byte_Buffer; //Devolvemos el array de bytes que contiene el mensaje codificado

        }
        public ACK Decodificar(byte[] byteMensaje) //Toma un array de bytes como entrada y devuelve un objeto ACK
        {
            MemoryStream ms = new MemoryStream(byteMensaje); //MemoryStream asociado al array de bytes recibido           
            BinaryReader reader = new BinaryReader(ms); //Lee el array de bytes del memory stream

            int ack_numero_secuencia = reader.ReadInt32(); //Lee entero de 32 bits del flujo de datos que corresponde al numero de secuencia del ACK recibido

            ACK Ack_recivido = new ACK(ack_numero_secuencia); //Crea un objeto ACK con el numero de secuencia recibido

            return Ack_recivido; //Devuelve el objeto que representa al ACK recibido
        }
    }
}