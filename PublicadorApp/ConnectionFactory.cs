using RabbitMQ.Client;
using System;

namespace AppPublicador
{
    public class ConnectionFactory
    {
        public static IConnection GetConnection()
        {
            var factory = new RabbitMQ.Client.ConnectionFactory();

            var userName = "raquel";
            var password = "AEbIbGmymo";
            var vhost = "aspire-notebook";
            var hostName = "localhost";
            var port = "5672";
            var uri = $@"amqp://{userName}:{password}@{hostName}:{port}/{vhost}";

            factory.Uri = new Uri(uri);

            return factory.CreateConnection();
        }

    }

}
