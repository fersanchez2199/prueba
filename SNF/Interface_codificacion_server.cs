using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Vocabulario;

namespace Vocabulario
{
    public interface Interface_codificacion_servidor
    {
        byte[] Codificar(ACK mensaje); //Función que codifica el mensaje ACK enviado por el servidor
        Datos Decodificar(byte[] byteMensaje); //Función que decodifica el mensaje enviado por el cliente
    }
}
