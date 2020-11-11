using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GeoLocator.Core.DAL.ORM
{
    public class GeoCoordinateContext:DbContext
    {
        public GeoCoordinateContext(DbContextOptions<GeoCoordinateContext> options) : base(options)
        {

        }

        public DbSet<GeoCoordinateRecord> GeoCoordinateRecords { get; set; }
    }
}
