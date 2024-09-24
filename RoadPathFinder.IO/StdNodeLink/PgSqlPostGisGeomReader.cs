using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoadPathFinder.IO.StdNodeLink
{
    public class PgSqlPostGisGeomReader
    {
        private string _connectionString { get; }

        public PgSqlPostGisGeomReader(string connectionString)
        {
            _connectionString = connectionString;

            var d = new SmallGeometry.Euclidean.FlatPoint(12, 3, SmallGeometry.CoordinateSystem.Epsg5186);
        }
    }
}
