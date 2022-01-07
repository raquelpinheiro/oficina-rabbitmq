using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace AppConsumidor
{
    public class RecebimentoExchangeDefault
    {
        private static IConnection conexaoRabbitMQ = null;
        private static IModel canalComunicacao = null;
        private static string nomeConsumidor = "Consumidor 1";
        private static string fila = "Ingressos";

        public static void ReceberMensagens()
        {

            conexaoRabbitMQ = ConnectionFactory.GetConnection();

            canalComunicacao = conexaoRabbitMQ.CreateModel();

            canalComunicacao.QueueDeclare(queue: fila,
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

            // prefetchCount: recebe do Broker apenas uma mensagem por vez até enviar o ACK
            canalComunicacao.BasicQos(prefetchSize: 0, prefetchCount: 10, global: false);

            Console.WriteLine($" [{nomeConsumidor}] - Aguardando mensagens...");

            var consumidor = new EventingBasicConsumer(canalComunicacao);

            canalComunicacao.BasicConsume(queue: fila,
                                           autoAck: false,
                                           consumerTag: $"{nomeConsumidor}",
                                           noLocal: false,
                                           exclusive: false,
                                           arguments: null,
                                           consumer: consumidor);

            consumidor.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($" [{ea.ConsumerTag}] - Recebeu: {message}");

                canalComunicacao.BasicAck(ea.DeliveryTag, false);
            };

        }

    }
}
