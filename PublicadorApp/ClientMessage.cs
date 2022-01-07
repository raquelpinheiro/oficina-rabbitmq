using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPublicador
{
    public class ClientMessage
    {
        private static IModel channel;
        public string Exchange { get; private set; }
        public string RoutingKey { get; private set; }

        public ClientMessage(string exchange, string routingKey)
        {
            Exchange = exchange;
            RoutingKey = routingKey;

            channel = ConnectionFactory.GetConnection().CreateModel();

            channel.ConfirmSelect();

            channel.BasicAcks += (sender, ea) =>
            {
                Console.WriteLine("OK - Mensagem recebida e encaminhada pelo Broker");
            };
            channel.BasicNacks += (sender, ea) =>
            {
                Console.WriteLine("ERRO - Mensagem NAO foi recebida e encaminhada pelo Broker");
            };
        }

        public void PostMessage(string mensagem)
        {
            var contentBody = Encoding.UTF8.GetBytes(mensagem);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true; // Se setado para true: a mensagem sobrevive na reinicialização do Broker

            channel.BasicPublish(exchange: Exchange, // Exchange Padrão: vai utilizar o Exchange (criado pelo RabbitMQ) com o mesmo da Fila
                                routingKey: RoutingKey, // nome da fila
                                mandatory: true, // Se setado para true: em caso de falha, a mensagem é descartada ou re-publicada para outro Exchange
                                basicProperties: properties,
                                body: contentBody);
        }

        public string PostMessageWithReply(string mensagem)
        {

            var args = new Dictionary<string, object>();
            args.Add("x-expires", 30000); // a fila vai durar apenas os segundos informados     

            // Fila temporária para armazenar a resposta da mensagem                
            var filaResposta = channel.QueueDeclare(exclusive: false, autoDelete: false, arguments: args).QueueName;

            IBasicProperties properties = channel.CreateBasicProperties();
            //properties.CorrelationId = mensagem.Id;

            properties.ReplyTo = filaResposta; // fila que o Publicador quer esperar a resposta
            var contentBody = Encoding.UTF8.GetBytes(mensagem);

            channel.BasicPublish(exchange: Exchange,
                                 routingKey: RoutingKey,
                                 basicProperties: properties,
                                 body: contentBody);

            return filaResposta;
        }

        public static TaskCompletionSource<string> GetResponse(string filaResposta, out string consumerTag)
        {
            TaskCompletionSource<string> taskResponse = new TaskCompletionSource<string>();

            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            var consumer = new EventingBasicConsumer(channel);

            consumerTag = channel.BasicConsume(queue: filaResposta,
                                               autoAck: false,
                                               consumer: consumer);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                //var id = ea.BasicProperties.CorrelationId;
                taskResponse.SetResult(Encoding.UTF8.GetString(body));

                channel.BasicAck(ea.DeliveryTag, false);
            };

            return taskResponse;
        }
    }
}
