using System;

namespace AppPublicador
{
    public class EnvioExchangeDirect
    {
        public static void EnviarMensagem(string mensagem, string periodo)
        {
            try
            {
                var helperRabbitMQ = new ClientMessage("IngressoPorDiaSemana", "Domingo");

                helperRabbitMQ.PostMessage(mensagem);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"### Erro: {ex.Message} {ex.InnerException?.Message} ###");
            }

        }
    }
}
