using System;

namespace Dominos.Core
{
    public class CoordinateDTO
    {        
        public decimal Source_latitude { get; set; }
        public decimal Source_longitude { get; set; }
        public decimal Destination_latitude { get; set; }
        public decimal Destination_longitude { get; set; }
        public int Distance { get; set; }

    }
}
