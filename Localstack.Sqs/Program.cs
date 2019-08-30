using System;
using System.Linq;
using System.Threading.Tasks;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Localstack.Sqs
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("SQS test start");
            var amazonSqsConfig = new AmazonSQSConfig
            {
                //uncomment to use localstack
                ServiceURL = "http://localhost:4576",
            };

            AmazonSQSClient amazonSqsClient = new AmazonSQSClient(amazonSqsConfig);
            SqsTestClient client = new SqsTestClient(amazonSqsClient);

            string queueName = "WebToolsQueue";
            await client.CreateQueueIfNotExistsAsync(queueName);
            string queueUrl = await client.GetQueueUrlAsync(queueName);
            await client.SendMessageAsync(queueUrl, "Hi webtools!");
            string receivedMessage = await client.ReceiveMessageAsync(queueUrl);
            string receivedMessageNull = await client.ReceiveMessageAsync(queueUrl);
        }
    }
}