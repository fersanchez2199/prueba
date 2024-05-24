namespace Vocabulario
{
    public class ACK
    {
        private int _num_secuencia;   //Variable para guardar el número de secuencia del ACK

        public int num_secuencia
        {
            get { return _num_secuencia; }
            set { _num_secuencia = value; }
        }

        public ACK(int num_secuencia)
        {
            _num_secuencia = num_secuencia;
        }
    }
}