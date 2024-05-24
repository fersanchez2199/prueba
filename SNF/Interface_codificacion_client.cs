using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Vocabulario;

namespace Vocabulario
{
    public interface Interface_codificacion_client
    {
        byte[] Codificar(Datos mensaje); //Función que codifica el mensaje enviado el cliente
        ACK Decodificar(byte[] byteMensaje); //Función que decodifica el ACK enviado por el servidor
    }
}