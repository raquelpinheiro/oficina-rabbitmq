using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPublicador
{
    public class EnvioExchangeDefaultACK
    {

        public static void EnviarMensagem(string mensagem)
        {
            try
            {
                var helperRabbitMQ = new ClientMessage(string.Empty, "FilaTeste");

                helperRabbitMQ.PostMessage(mensagem);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"### Erro: {ex.Message} {ex.InnerException?.Message} ###");
            }

        }
    }
}
