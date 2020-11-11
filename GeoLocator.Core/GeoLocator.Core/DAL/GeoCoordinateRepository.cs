using Dominos.Core;
using EFCore.BulkExtensions;
using GeoLocator.Core.DAL.ORM;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GeoLocator.Core.DAL
{
    public class GeoCoordinateRepository : IGeoCoordinateRepository
    {
        private readonly GeoCoordinateContext _geoCoordinateContext;
        private readonly HttpClient _httpClient;

        public GeoCoordinateRepository(GeoCoordinateContext geoCoordinateContext)
        {
            _geoCoordinateContext = geoCoordinateContext;
        }
        public async Task<string> AddRecordsToDB(List<GeoCoordinateRecord> record)
        {
            bool result = true;
            long elapsed_time = 0;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                //record.gcInsertDate = DateTime.Now;
                //Transaction olmadan DB insert performans defecti yaratir !
                using (var trnx = _geoCoordinateContext.Database.BeginTransaction())
                {
                    //Bulk Insert ! Performans icin bu şart..
                    await _geoCoordinateContext.BulkInsertAsync(record);                                        

                    //await _geoCoordinateContext.SaveChangesAsync();
                    await trnx.CommitAsync();
                }

                stopwatch.Stop();
                elapsed_time = stopwatch.ElapsedMilliseconds;
            }
            catch (Exception ex)
            {
                result = false;
                return String.Format("ERROR at Insert : {0} \n" , ex.Message);
            }

            return String.Format("Insert completed at {0} ms\n" , elapsed_time.ToString() );
        }

        public async Task<string> AddRecordsToQueu(List<CoordinateDTO> record)
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    //RabbitMQ URL
                    Uri = new Uri("amqp://guest:guest@localhost:5672"),
                    DispatchConsumersAsync = true,
                    ConsumerDispatchConcurrency = 32
                };

                using var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();
                channel.QueueDeclare("demo-queue",
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                    );

                long elapsed_time = 0;
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                
                await Task.Run(() =>
                {
                    foreach (var item in record)
                    {
                        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item));
                        channel.BasicPublish("", "demo-queue", null, body);
                    }
                });            
                
                /*
                for (int i = 0; i < record.Count; i++)
                {
                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(record[i]));
                    channel.BasicPublish("", "demo-queue", null, body);
                }
                */
                
                stopwatch.Stop();
                elapsed_time = stopwatch.ElapsedMilliseconds;

                return String.Format("Queue completed at {0} ms", elapsed_time.ToString());
            }
            catch (Exception ex)
            {
                return String.Format("ERROR at Queue : {0} ", ex.Message);
            }
        }

        public Task<GeoCoordinateRecord> GetWeatherData(string location)
        {
            throw new NotImplementedException();
        }
    }
}
