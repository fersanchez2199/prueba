using System;
using Vocabulario;
namespace Test_Codificacion
{
    [TestClass]
    public class test_codificacion_ACK
    {
        [TestMethod]
        public void TestCodec_Server()
        {
            int secuecia = 4;
            ACK Test_ack = new ACK(secuecia);
            Codificacion_Binaria_Server test_codificacion = new Codificacion_Binaria_Server();
            byte[] codi = test_codificacion.Codificar(Test_ack);
            Codificacion_Binaria_Client test_decodificacion = new Codificacion_Binaria_Client();
            ACK resultado_ack = test_decodificacion.Decodificar(codi);
            int resultado_secuencia = resultado_ack.num_secuencia;
            Assert.AreEqual(secuecia, resultado_secuencia);
            Console.WriteLine(secuecia + "->secuencias a codificar || " + resultado_secuencia + "-> número de secuencias decodificado");
        }
        
    }
    
}