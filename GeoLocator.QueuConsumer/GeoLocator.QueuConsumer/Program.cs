using Dominos.Core;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NLog;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GeoLocator.QueuConsumer
{
    class Program
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        static object locker = new object();

        static void Main(string[] args)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri("amqp://guest:guest@localhost:5672"),
                    ConsumerDispatchConcurrency = 32,  // max 32 kanal paralel calisma oalbilsin.
                    DispatchConsumersAsync = true  //Async calisabilme icin 
                };

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.QueueDeclare("demo-queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                    );

                var consumer = new AsyncEventingBasicConsumer(channel);  // TPL icin..

                logger.Log(NLog.LogLevel.Info, "S_latitude|S_longitude|D_latitude|D_longitude|Distance");
                logger.Log(NLog.LogLevel.Info, " ");
                consumer.Received += async (sender, e) => {
                    var body = e.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    CoordinateDTO aa = JsonConvert.DeserializeObject<CoordinateDTO>(message);
                    Console.WriteLine(aa.Source_latitude + " | " + aa.Source_longitude + " | " + aa.Destination_latitude + " | " + aa.Destination_longitude + " | " + aa.Distance);

                    logger.Log(NLog.LogLevel.Info, aa.Source_latitude + " | " + aa.Source_longitude + " | " + aa.Destination_latitude + " | " + aa.Destination_longitude + " | " + aa.Distance);
                    //await WriteTextAsync(aa.Source_latitude + " | " + aa.Source_longitude + " | " + aa.Destination_latitude + " | " + aa.Destination_longitude + " | " + aa.Distance);
                };

                channel.BasicConsume("demo-queue", true, consumer);
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                lock (locker)
                {
                    logger.Error(ex, "ERROR at Main");
                }
            }
        }

        static async Task WriteTextAsync(string text)
        {
            try
            {
                byte[] encodedText = Encoding.Unicode.GetBytes(text);
                using (FileStream sourceStream = new FileStream("LogFileAsync.txt", FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096 * 64, useAsync: true))
                {
                    await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
                };
            }
            catch (Exception ex)
            {
                //logger.Error(ex, "ERROR at Main");
            }
        }
    }
}
