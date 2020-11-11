using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GeoLocator.Core
{
    public class GeoCoordinateRecord
    {
        [Key]
        public int gcId { get; set; }
        public decimal gcSource_latitude { get; set; }
        public decimal gcSource_longitude { get; set; }
        public decimal gcDestination_latitude { get; set; }
        public decimal gcDestination_longitude { get; set; }
        public DateTime gcInsertDate { get; set; }

    }
}
