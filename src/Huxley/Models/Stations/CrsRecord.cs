/*
Huxley - a JSON proxy for the UK National Rail Live Departure Board SOAP API
Copyright (C) 2016 James Singleton
 * https://huxley.unop.uk
 * https://github.com/jpsingleton/Huxley

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using GeoCoordinatePortable;
using GeoUK.Coordinates;

namespace Huxley {
    public class CrsRecord
    {
        public string StationName { get; set; }
        public string CrsCode { get; set; }     
    }

    public class NaptaStationRecord : CrsRecord
    {
        public string Easting { get; set; }
        public string Northing { get; set; }
    }

    public class NREStationRecord : CrsRecord {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class NREStationLocationRecord : NREStationRecord {
        public double Distance { get; set; }
    }

    public class DistanceComparer : IComparer<NREStationRecord> {
      
        public GeoCoordinatePortable.GeoCoordinate TargetLocation { get; set; }

        public int Compare(NREStationRecord x, NREStationRecord y) {

            GeoCoordinatePortable.GeoCoordinate xlocation = new GeoCoordinate(x.Latitude, x.Longitude);
            GeoCoordinatePortable.GeoCoordinate ylocation = new GeoCoordinate(y.Latitude, y.Longitude);

            double diff = xlocation.GetDistanceTo(TargetLocation) - ylocation.GetDistanceTo(TargetLocation);
            return (int) diff;
        }
    }
}