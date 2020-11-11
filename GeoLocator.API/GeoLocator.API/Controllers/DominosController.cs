using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GeoLocator.Core;
using GeoLocator.Core.DAL;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;
using System.Text;
using GeoCoordinatePortable;
using Dominos.Core;

namespace GeoLocator.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DominosController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private IMemoryCache _memoryCache;
        private readonly ILogger<DominosController> _logger;
        private readonly IGeoCoordinateRepository _geoCoordinateRepository;

        public DominosController(ILogger<DominosController> logger, IGeoCoordinateRepository geoCoordinateRepository)
        {
            _logger = logger;
            _geoCoordinateRepository = geoCoordinateRepository;
        }

        //[HttpGet]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    var rng = new Random();
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = rng.Next(-20, 55),
        //        Summary = Summaries[rng.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}


        [HttpGet("DummyInsert")]
        public async Task<ActionResult<string>> Get()
        {
            var res = "";
            try
            {
                // KEY cache te tutulacak unique key uret :
                //string memCacheKey = (geoCoordinateRecord.destination_latitude.ToString() + geoCoordinateRecord.destination_longitude.ToString()
                //    + geoCoordinateRecord.source_latitude.ToString() + geoCoordinateRecord.source_longitude.ToString()).Trim().ToLower();


                var cacheExpOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpiration = DateTime.Now.AddMinutes(60), //60dk cachete tut
                    Size = 50
                };

                // 1.KONTROL: Cache e bak
                //if (!_memoryCache.TryGetValue(memCacheKey, out GeoCoordinateRecord cachedWeatherRecord)) //cachte kayit yok ise
                {
                    //var dbRecord = await _geoCoordinateRepository.GetWeatherData(memCacheKey);
                    List<GeoCoordinateRecord> recordList = new List<GeoCoordinateRecord>();
                    List<CoordinateDTO> recordListForQueu = new List<CoordinateDTO>();
                    GeoCoordinateRecord rec;
                    CoordinateDTO Queu;
                    Random myRandom;

                    { }
                    for (int i = 0; i < 10000; i++)
                    {
                        myRandom = new Random();
                        rec = new GeoCoordinateRecord();
                        Queu = new CoordinateDTO();

                        //--------------------  Colelction for DB Insert -----------------------------------
                        rec.gcSource_latitude = Convert.ToDecimal(String.Format("{0:0.000000}", myRandom.NextDouble(36.000000, 42.000000)));
                        rec.gcSource_longitude = Convert.ToDecimal(String.Format("{0:0.000000}", myRandom.NextDouble(26.000000, 45.000000)));
                        rec.gcDestination_latitude = Convert.ToDecimal(String.Format("{0:0.000000}", myRandom.NextDouble(36.000000, 42.000000)));
                        rec.gcDestination_longitude = Convert.ToDecimal(String.Format("{0:0.000000}", myRandom.NextDouble(26.000000, 45.000000)));
                        rec.gcInsertDate = DateTime.Now;
                        recordList.Add(rec);
                        //-----------------------------------------------------------------------------------

                        //--------------------  Colelction for Log Queu with distance in KM  ----------------
                        Queu.Source_latitude = rec.gcSource_latitude;
                        Queu.Source_longitude = rec.gcSource_longitude;
                        Queu.Destination_latitude = rec.gcDestination_latitude;
                        Queu.Destination_longitude = rec.gcDestination_longitude;
                        //Calculate Distance between 2 points 
                        var locA = new GeoCoordinate(Convert.ToDouble(rec.gcSource_latitude), Convert.ToDouble(rec.gcSource_longitude));
                        var locB = new GeoCoordinate(Convert.ToDouble(rec.gcDestination_latitude), Convert.ToDouble(rec.gcDestination_longitude));
                        double distance = locA.GetDistanceTo(locB) / 1000; // kilometres
                        Queu.Distance = Convert.ToInt32(distance);
                        recordListForQueu.Add(Queu);
                        //-----------------------------------------------------------------------------------
                    }

                    // DB Insert
                    res = await _geoCoordinateRepository.AddRecordsToDB(recordList);

                    // Queu Insert
                    res += await _geoCoordinateRepository.AddRecordsToQueu(recordListForQueu);
                }
                //else // Kayit Cache de mevcut ise bunu kullan..
                {

                }

                return Ok(res);
            }
            catch (Exception ex)
            {
                return NotFound("ERROR at Try(GetWeatherData): " + ex.Message);
            }
        }
    }

    public static class RandomExtensions
    {
        public static double NextDouble(this Random random, double minValue, double maxValue)
        {
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }
    }
}
