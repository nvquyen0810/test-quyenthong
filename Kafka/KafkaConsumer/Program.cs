using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace KafkaConsumer
{
    class Program
    {
        static void TaskEmail(HttpClient client, StringContent data,
            IConsumer<string, string> consumer, ConsumeResult<string, string> cr)
        {
            List<TopicPartitionOffset> list = new List<TopicPartitionOffset>();
            var offset = cr.TopicPartitionOffset;
            list.Add(offset);

            var urlEmail = "https://localhost:44362/api/v1/orders/create-history-email";
            object messagePrintConsole = "";
            try
            {
                var responseEmail = client.PostAsync(urlEmail, data);
                consumer.Commit(cr);
                var contentsEmail = responseEmail.Result.Content.ReadAsStringAsync().Result;
                var finalResultEmail = Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(contentsEmail);

                // consumer.Commit(list cr.TopicPartitionOffset)
                messagePrintConsole = finalResultEmail.Data;
            }
            finally
            {
                Console.WriteLine("HistoryEmailID: " + messagePrintConsole + ", Commited offset: " + offset);
            }
        }

        static async Task TaskEmailAsync2(HttpClient client, StringContent data,
            IConsumer<string, string> consumer, ConsumeResult<string, string> cr)
        {
            List<TopicPartitionOffset> list = new List<TopicPartitionOffset>();
            var offset = cr.TopicPartitionOffset;
            list.Add(offset);

            var urlEmail = "https://localhost:44362/api/v1/orders/create-history-email";
            object messagePrintConsole = "";
            try
            {
                var responseEmail = await client.PostAsync(urlEmail, data);
                if (responseEmail.IsSuccessStatusCode)
                {
                    consumer.Commit(cr);
                    var contentsEmail = responseEmail.Content.ReadAsStringAsync().Result;
                    var finalResultEmail = Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(contentsEmail);
                    Console.WriteLine("HistoryEmailID: " + messagePrintConsole + ", Commited offset: " + offset);
                }
            }
            catch (ConsumeException e)
            {
                Console.WriteLine($"Error occured: {e.Error.Reason}");
            }

        }

        static void TaskPdf(HttpClient client, StringContent data,
            IConsumer<string, string> consumer, ConsumeResult<string, string> cr)
        {
            List<TopicPartitionOffset> list = new List<TopicPartitionOffset>();
            var offset = cr.TopicPartitionOffset;
            list.Add(offset);

            var urlPdf = "https://localhost:44362/api/v1/orders/create-history-pdf";
            object messagePrintConsole = "";
            try
            {
                var responsePdf = client.PostAsync(urlPdf, data);
                consumer.Commit(cr);
                var contentsPdf = responsePdf.Result.Content.ReadAsStringAsync().Result;
                var finalResultPdf = Newtonsoft.Json.JsonConvert.DeserializeObject<Response>(contentsPdf);
                messagePrintConsole = finalResultPdf.Data;
            }
            finally
            {
                // consumer.Commit(list cr.TopicPartitionOffset)
                Console.WriteLine("HistoryPdfID: " + messagePrintConsole + ", Commited offset: " + offset);
            }


        }

        //static void PrintUsage()
        //{
        //    Console.WriteLine("usage: .. produce|consume <topic> <configPath> [<certDir>]");
        //    System.Environment.Exit(1);
        //}

        static void Main(string[] args)
        {
            var consumerConfig = new ConsumerConfig
            {
                GroupId = "fptecommerce_consumer_group",
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest,

                EnableAutoCommit = false,
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

                        try
                        {
                            if (string.IsNullOrEmpty(cr?.Message?.Value)) continue;
                            //if (cr.IsPartitionEOF || cr.Message.Value == null) continue;
                            //if (string.IsNullOrEmpty(cr?.Message?.Value)) continue;


                            KafkaMessage? messageValue = JsonSerializer.Deserialize<KafkaMessage>(cr.Message.Value);
                            //Console.WriteLine($"Key: {cr.Message.Key}, Message: ({messageValue.OrderId}), received from {cr.TopicPartitionOffset}");


                            // Nếu key ~ gửi email
                            if (cr.Message.Key == "OrderIdAndTokenEmail")
                            {
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


                                Task t1 = new Task(() => TaskEmail(client, data, consumer, cr));
                                t1.Start();
                            }


                            // Nếu key ~ tạo pdf
                            if (cr.Message.Key == "OrderIdAndTokenPdf")
                            {
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


                                Task t2 = new Task(() => TaskPdf(client, data, consumer, cr));
                                t2.Start();
                            }
                        }
                        catch (ConsumeException e)
                        {
                            Console.WriteLine($"Error occured: {e.Error.Reason}");
                        }
                        finally
                        {

                            //consumer.Commit(cr);
                            //consumer.Commit(list);
                        }

                        //KafkaMessage? messageValue = JsonSerializer.Deserialize<KafkaMessage>(cr.Message.Value);

                        //Console.WriteLine($"Key: {cr.Message.Key}, Message: ({messageValue.OrderId}), received from {cr.TopicPartitionOffset}");


                        /*
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

                        // Cách 1
                        //Thread t1 = new Thread(() => TaskEmail(client, data));
                        //Thread t2 = new Thread(() => TaskPdf(client, data));
                        //t1.Start();
                        //t2.Start();

                        // Cách 2
                        Task t1 = new Task(() => TaskEmail(client, data));
                        Task t2 = new Task(() => TaskPdf(client, data));
                        t1.Start();
                        t2.Start();
                        */
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
