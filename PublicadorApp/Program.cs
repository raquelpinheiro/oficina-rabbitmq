using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppPublicador
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var lugarReservado = EnvioExchangeDirectComResposta.EnviarMensagem("Ingresso da Raquel");

            Console.WriteLine($"Lugar reservado: {lugarReservado}");

            // EnvioExchangeDirect.EnviarMensagem("Quero um ingresso para domingo: Raquel", "Domingo");

            // EnvioExchangeDefaultACK.EnviarMensagem("Ingresso da Raquel");

            //EnvioExchangeDefault.EnviarMensagem("Raquel");

            //EnvioExchangeDefaultACK.EnviarMensagem("Raquel");

            Console.ReadLine();

        }

    }
}
