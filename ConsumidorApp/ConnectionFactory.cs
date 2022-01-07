using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppConsumidor
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
