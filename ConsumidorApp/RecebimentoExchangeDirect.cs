using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;

namespace AppConsumidor
{
    public class RecebimentoExchangeDirect
    {
        private static string nomeConsumidor = "Consumidor 1";
        private static IConnection conexaoRabbitMQ = null;
        private static IModel canalComunicacao = null;
        private static string exchange = "IngressoPorPeriodoDia";
        private static string fila = "IngressosMatrixTarde";
        private static string filaIngressosManha = "IngressosMatrixManha";

        public static void ReceberMensagens()
        {

            conexaoRabbitMQ = ConnectionFactory.GetConnection();

            canalComunicacao = conexaoRabbitMQ.CreateModel();

            // Declara o Exchange
            canalComunicacao.ExchangeDeclare(exchange: exchange,
                                             type: ExchangeType.Direct,
                                             durable: true, autoDelete: false, arguments: null);

            // Declara as Filas
            canalComunicacao.QueueDeclare(queue: fila,
                                        durable: true,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);
            // Declara as Filas
            canalComunicacao.QueueDeclare(queue: filaIngressosManha,
                                        durable: true,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);
            
            // Faz a ligação da Fila com o Exchange
            canalComunicacao.QueueBind(queue: fila,
                                       exchange: exchange,
                                       routingKey: "Tarde",
                                       arguments: null);

            canalComunicacao.QueueBind(queue: filaIngressosManha,
                                 exchange: exchange,
                                 routingKey: "Manha",
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

                Console.WriteLine($" [{ea.ConsumerTag}] - Recebeu a mensagem: {message}");

                Thread.Sleep(5000);

                canalComunicacao.BasicAck(ea.DeliveryTag, false);

                Console.WriteLine($" [{ea.ConsumerTag}] - Conclui a mensagem: {message}");
            };

        }

    }
}
