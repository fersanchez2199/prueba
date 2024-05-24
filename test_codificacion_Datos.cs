using System;
using Vocabulario;
namespace Test_Codificacion
{
    [TestClass]
    public class test_codificacion_Datos
    {
        [TestMethod]
        public void TestCodec_Client()
        {
            int secuecia = 4;
            byte mensaje = 0;
            Datos Test_mensaje = new Datos(mensaje, secuecia);
            Codificacion_Binaria_Client test_codificacion = new Codificacion_Binaria_Client();
            byte[] codi = test_codificacion.Codificar(Test_mensaje);
            Codificacion_Binaria_Server test_decodificacion = new Codificacion_Binaria_Server();
            Datos resultado_mensaj = test_decodificacion.Decodificar(codi);
            int resultado_secuencia = resultado_mensaj.num_secuencia;
            byte mensaj_resultado = resultado_mensaj.mensaje;
            Assert.AreEqual(mensaje, mensaj_resultado);
            Console.WriteLine("Mensaje antes de codificar ->" + mensaje + " Mensaje decodificado -> " + mensaj_resultado);
            Assert.AreEqual(secuecia, resultado_secuencia);
            Console.WriteLine("Número de secuencia antes de codificar -> " + secuecia + " Número de secuencia decodificado -> " + resultado_secuencia);
        }

    }
}