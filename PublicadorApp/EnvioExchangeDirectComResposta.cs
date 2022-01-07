using System.Threading;

namespace AppPublicador
{
    public class EnvioExchangeDirectComResposta
    {
        public static int EnviarMensagem(string mensagem)
        {
            int lugarReservado = 0;
            var helperRabbitMQ = new ClientMessage("IngressoPorDiaSemana", "Domingo");

            var filaResposta = helperRabbitMQ.PostMessageWithReply(mensagem);

            var taskCompletion = ClientMessage.GetResponse(filaResposta, out string identificadorConsumidor);
            var taskResposta = taskCompletion.Task;

            CancellationToken cancellationToken = default(CancellationToken);
            taskResposta.Wait(10000, cancellationToken);

            if (taskResposta.IsCompleted)
            {
                int.TryParse(taskResposta.Result, out lugarReservado);
            }
            else
            {
                taskCompletion.TrySetResult("");
            }

            return lugarReservado;
        }

    }
}
