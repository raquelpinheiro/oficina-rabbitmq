using System;
using System.Text;

namespace AppPublicador
{
    public class EnvioExchangeDefault
    {
        public static void EnviarMensagem(string mensagem)
        {
            try
            {
                using (var connection = ConnectionFactory.GetConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        var contentBody = Encoding.UTF8.GetBytes(mensagem);

                        var properties = channel.CreateBasicProperties();
                        properties.Persistent = true; // Se setado para true: a mensagem sobrevive na reinicialização do Broker

                        channel.BasicPublish(exchange: string.Empty, // Exchange Padrão: vai utilizar o Exchange (criado pelo RabbitMQ) com o mesmo da Fila
                                            routingKey: "Ingressos",
                                            mandatory: true, // Se setado para true: em caso de falha, a mensagem é descartada ou re-publicada para outro Exchange
                                            basicProperties: properties,
                                            body: contentBody);
                    }

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"### Erro: {ex.Message} {ex.InnerException?.Message} ###");
            }

        }
    }
}
