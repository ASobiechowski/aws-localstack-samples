using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

namespace Localstack.Dynamo
{
    class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Start DynamoDb test");

            AmazonDynamoDBConfig amazonDynamoDbConfig = new AmazonDynamoDBConfig
            {
                //uncomment to use localstack
                ServiceURL = "http://localhost:4569"
            };

            var amazonDynamoDbClient = new AmazonDynamoDBClient(amazonDynamoDbConfig);

            DynamoTestClient testClient = new DynamoTestClient(amazonDynamoDbClient);

            await testClient.CreateTableIfNotExistsAsync();
            var id = await testClient.PutItemAsync("Hi webtools!");
            var message = await testClient.GetItemMessageAsync(id);

            Console.WriteLine($"Inserted item message: {message}");
        }
    }
}