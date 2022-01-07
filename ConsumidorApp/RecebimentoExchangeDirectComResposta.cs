using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace AppConsumidor
{
    public class RecebimentoExchangeDirectComResposta
    {

        private static string nomeConsumidor = "Consumidor - Raquel";
        private static IConnection conexaoRabbitMQ = null;
        private static IModel canalComunicacao = null;
        private static string Exchange = "IngressoPorDiaSemana";
        private static int Contador = 0;
        private static string Fila = "IngressosDomingo";

        public static void ReceberMensagens()
        {

            conexaoRabbitMQ = ConnectionFactory.GetConnection();

            canalComunicacao = conexaoRabbitMQ.CreateModel();

            // Declara o Exchange
            canalComunicacao.ExchangeDeclare(exchange: Exchange,
                                             type: ExchangeType.Direct,
                                             durable: true, autoDelete: false, arguments: null);

            // Declara as Filas
            canalComunicacao.QueueDeclare(queue: Fila,
                                        durable: true,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

            // Faz a ligação da Fila com o Exchange
            canalComunicacao.QueueBind(queue: Fila,
                                       exchange: Exchange,
                                       routingKey: "Domingo",
                                       arguments: null);


            // prefetchCount: recebe do Broker a quantidade de mensagens sem ter enviado o ACK de uma mensagem
            canalComunicacao.BasicQos(prefetchSize: 0, prefetchCount: 10, global: false);

            Console.WriteLine($" [{nomeConsumidor}] - Aguardando mensagens...");

            var consumidor = new EventingBasicConsumer(canalComunicacao);

            canalComunicacao.BasicConsume(queue: Fila,
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

                Console.WriteLine($" [{ea.ConsumerTag}] - Recebeu a mensagem: {message}");

                // simulando o processamento de alguma coisa
                Thread.Sleep(5000);

                // envio do ACK manual
                canalComunicacao.BasicAck(ea.DeliveryTag, false);

                Console.WriteLine($" [{ea.ConsumerTag}] - Conclui a mensagem: {message}");

                // Envio da resposta para a Fila Temporária que o Publicador está esperando
                var props = ea.BasicProperties;
                var replyProps = canalComunicacao.CreateBasicProperties();

                var senha = Interlocked.Increment(ref Contador);

                var resposta = Encoding.UTF8.GetBytes(senha.ToString());

                canalComunicacao.BasicPublish(exchange: "",
                           routingKey: props.ReplyTo,
                           mandatory: false,
                           basicProperties: replyProps,
                           body: resposta);
            };

        }

    }
}
