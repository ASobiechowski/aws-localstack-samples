using System.Linq;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Localstack.Sqs
{
    public class SqsTestClient
    {
        private readonly AmazonSQSClient _client;

        public SqsTestClient(AmazonSQSClient client)
        {
            _client = client;
        }

        public async Task CreateQueueIfNotExistsAsync(string queueName)
        {
            var createQueueRequest = new CreateQueueRequest(queueName);
            await _client.CreateQueueAsync(createQueueRequest);
        }

        public async Task<string> GetQueueUrlAsync(string queueName)
        {
            var queues = await _client.ListQueuesAsync(queueName);
            return queues.QueueUrls.FirstOrDefault();
        }

        public async Task SendMessageAsync(string queueUrl, string message)
        {
            var sendMessageRequest =
                new SendMessageRequest
                {
                    QueueUrl = queueUrl,
                    MessageBody = message,
                };

            var sendResult = await _client.SendMessageAsync(sendMessageRequest);
        }

        public async Task<string> ReceiveMessageAsync(string queueUrl)
        {
            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queueUrl
            };

            var receiveMessageResponse =
                await _client.ReceiveMessageAsync(receiveMessageRequest);
            return receiveMessageResponse.Messages.FirstOrDefault()?.Body;
        }

    }
}