using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Model
{
    internal class DataModel
    {
        public class Coordinate
        {
            public int id { get; set; }
            public double lon { get; set; }
            public double lat { get; set; }
            public string district { get; set; }
            public string description { get; set; }
        }

        public class EnvironmentalDataEntry
        {
            public Guid id { get; set; }
            public int coordinateId { get; set; } // Foreign key
            public long dt { get; set; }
            public DateTime dateTime { get; set; }
            public double temp { get; set; }
            public double feelsLike { get; set; }
            public int pressure { get; set; }
            public int humidity { get; set; }
            public double tempMin { get; set; }
            public double tempMax { get; set; }
            public int aqi { get; set; }
            public double co { get; set; }
            public double no { get; set; }
            public double no2 { get; set; }
            public double o3 { get; set; }
            public double so2 { get; set; }
            public double pm2_5 { get; set; }
            public double pm10 { get; set; }
            public double nh3 { get; set; }
        }

    }
}
