namespace Vocabulario
{
    public class Datos
    {
        private int _num_secuencia;   //Variable para guardar el número de secuencia del mensaje
        private byte _mensaje;    //Variable para guardar el mensaje convertido en bytes

        public int num_secuencia
        {
            get { return _num_secuencia; }
            set { _num_secuencia = value; }
        }
        public byte mensaje
        {
            get { return _mensaje; }
            set { _mensaje = value; }
        }

        public Datos(byte mensaje, int num_secuencia)
        {
            _num_secuencia = num_secuencia;
            _mensaje = mensaje;
        }
    }
}