using Confluent.Kafka;
using System;
using System.Text.Json;
using System.Threading;

namespace KafkaConsumer
{
    class Program
    {
        static void Consume(string topic)
        {
            var consumerConfig = new ConsumerConfig
            {
                GroupId = "fptecommerce_consumer_group",
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            CancellationTokenSource cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true; // prevent the process from terminating.
                cts.Cancel();
            };

            using (var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build())
            {
                consumer.Subscribe(topic);
                try
                {
                    while (true)
                    {
                        var cr = consumer.Consume(cts.Token);

                        KafkaMessage? messageValue = JsonSerializer.Deserialize<KafkaMessage>(cr.Message.Value);

                        Console.WriteLine($"Key: {cr.Message.Key}, Message: ({messageValue.OrderId}), received from {cr.TopicPartitionOffset}");
                    }
                }
                catch (OperationCanceledException)
                {
                    // Ctrl-C was pressed.
                }
                finally
                {
                    consumer.Close();
                }
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("usage: .. produce|consume <topic> <configPath> [<certDir>]");
            System.Environment.Exit(1);
        }

        static void Main(string[] args)
        {
            var config = new ConsumerConfig
            {
                GroupId = "fptecommerce_consumer_group",
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            Consume("fptecommerce_topic");
        }
    }
}
