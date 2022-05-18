using Confluent.Kafka;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace KafkaConsumer
{
    class Program
    {
        static void TaskEmail(HttpClient client, StringContent data)
        {
            var urlEmail = "https://localhost:44362/api/v1/orders/create-history-email";

            var responseEmail = client.PostAsync(urlEmail, data);
            var contentsEmail = responseEmail.Result.Content.ReadAsStringAsync().Result;
            var finalResultEmail = Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(contentsEmail);
            Console.WriteLine("HistoryEmailID: " + finalResultEmail.Data);
        }

        static void TaskPdf(HttpClient client, StringContent data)
        {
            var urlPdf = "https://localhost:44362/api/v1/orders/create-history-pdf";

            var responsePdf = client.PostAsync(urlPdf, data);
            var contentsPdf = responsePdf.Result.Content.ReadAsStringAsync().Result;
            var finalResultPdf = Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(contentsPdf);
            Console.WriteLine("HistoryPdfID: " + finalResultPdf.Data);
        }
        //static void Consume(string topic)
        //{
        //    var consumerConfig = new ConsumerConfig
        //    {
        //        GroupId = "fptecommerce_consumer_group",
        //        BootstrapServers = "localhost:9092",
        //        AutoOffsetReset = AutoOffsetReset.Earliest
        //    };

        //    CancellationTokenSource cts = new CancellationTokenSource();
        //    Console.CancelKeyPress += (_, e) =>
        //    {
        //        e.Cancel = true; // prevent the process from terminating.
        //        cts.Cancel();
        //    };

        //    var client = new HttpClient();

        //    using (var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build())
        //    {
        //        consumer.Subscribe("fptecommerce_topic");
        //        try
        //        {
        //            while (true)
        //            {
        //                var cr = consumer.Consume(cts.Token);

        //                KafkaMessage? messageValue = JsonSerializer.Deserialize<KafkaMessage>(cr.Message.Value);

        //                //Console.WriteLine($"Key: {cr.Message.Key}, Message: ({messageValue.OrderId}), received from {cr.TopicPartitionOffset}");



        //                var kafkaMessage = new CreateHistoryDTO()
        //                {
        //                    OrderId = messageValue.OrderId
        //                };
        //                var messageKaf = JsonSerializer.Serialize<CreateHistoryDTO>(kafkaMessage);

        //                var data = new StringContent(messageKaf, Encoding.UTF8, "application/json");

        //                //string accessToken = @"eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJVc2VyTmFtZSI6InRoYW5oIiwiRW1haWwiOiJ2ZWdldGEubWluaHRoYW5oQGdtYWlsLmNvbSIsIklkIjoiNSIsIlRva2VuSWQiOiJhNTU0OGE3ZC1mYTYxLTRhOGMtYTI2ZS1hNTE2MjgyMWI5MWMiLCJuYmYiOjE2NTI4NzkxNzksImV4cCI6MTY1Mjk2NTU3OSwiaWF0IjoxNjUyODc5MTc5fQ.DjaZ67ILRdBk7YaY0kNmxxKUe1gV27HgWnCGHZ94RM3JP5ogeVJRQ2rSXhuaxrBBWPytMCVxP6rAJ1WCX_2FZg";
        //                string accessToken = messageValue.Token;
        //                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);


        //                Thread t1 = new Thread(() => TaskEmail(client, data));
        //                Thread t2 = new Thread(() => TaskPdf(client, data));
        //                t1.Start();
        //                t2.Start();


        //            }
        //        }
        //        catch (OperationCanceledException)
        //        {
        //            // Ctrl-C was pressed.
        //        }
        //        finally
        //        {
        //            consumer.Close();
        //        }
        //    }
        //}

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

            //Consume("fptecommerce_topic");
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

            var client = new HttpClient();

            using (var consumer = new ConsumerBuilder<string, string>(consumerConfig).Build())
            {
                consumer.Subscribe("fptecommerce_topic");
                try
                {
                    while (true)
                    {
                        var cr = consumer.Consume(cts.Token);

                        KafkaMessage? messageValue = JsonSerializer.Deserialize<KafkaMessage>(cr.Message.Value);

                        //Console.WriteLine($"Key: {cr.Message.Key}, Message: ({messageValue.OrderId}), received from {cr.TopicPartitionOffset}");



                        var kafkaMessage = new CreateHistoryDTO()
                        {
                            OrderId = messageValue.OrderId
                        };
                        var messageKaf = JsonSerializer.Serialize<CreateHistoryDTO>(kafkaMessage);

                        var data = new StringContent(messageKaf, Encoding.UTF8, "application/json");

                        //string accessToken = @"eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJVc2VyTmFtZSI6InRoYW5oIiwiRW1haWwiOiJ2ZWdldGEubWluaHRoYW5oQGdtYWlsLmNvbSIsIklkIjoiNSIsIlRva2VuSWQiOiJhNTU0OGE3ZC1mYTYxLTRhOGMtYTI2ZS1hNTE2MjgyMWI5MWMiLCJuYmYiOjE2NTI4NzkxNzksImV4cCI6MTY1Mjk2NTU3OSwiaWF0IjoxNjUyODc5MTc5fQ.DjaZ67ILRdBk7YaY0kNmxxKUe1gV27HgWnCGHZ94RM3JP5ogeVJRQ2rSXhuaxrBBWPytMCVxP6rAJ1WCX_2FZg";
                        string accessToken = messageValue.Token;

                        //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                        Thread t1 = new Thread(() => TaskEmail(client, data));
                        Thread t2 = new Thread(() => TaskPdf(client, data));
                        t1.Start();
                        t2.Start();


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
    }
}
