using Dominos.Core;
using GeoLocator.Core.DAL;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GeoLocator.Core.DAL
{
    public interface IGeoCoordinateRepository
    {
        Task<GeoCoordinateRecord> GetWeatherData(string location);
        Task<string> AddRecordsToDB(List<GeoCoordinateRecord> record);
        Task<string> AddRecordsToQueu(List<CoordinateDTO> record);
    }
}
