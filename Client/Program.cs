using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Security;
using System.Threading;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Client _Cliente = new Client();
            _Cliente.Run();
        }
    }
}