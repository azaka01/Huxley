using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using GeoUK.Coordinates;

namespace Huxley.Utils {
    public class LocationRequestHander {

        readonly DistanceComparer distanceComparer = new DistanceComparer();

        public LocationRequestHander() {
        }

        private double GetFromString(String val) {
            double result;
            if (Double.TryParse(val, out result)) {
                return result;
            }
         
            throw new HttpResponseException(HttpStatusCode.BadRequest);
        }

        public IEnumerable<NREStationRecord> GetNearestStations(String location, IList<NREStationRecord> allStations, int numresults) {

            var latlon = location.Split(',').ToArray();

            double lat = GetFromString(latlon[0]);
            double lon = GetFromString(latlon[1]);

            LatitudeLongitude l = new LatitudeLongitude(lat, lon);

            var inputCoordinates = new GeoCoordinatePortable.GeoCoordinate(l.Latitude, l.Longitude);

            distanceComparer.TargetLocation = inputCoordinates;
            var count = allStations.Count();
            if (numresults > 0) {
                count = numresults;
            }

            var filteredStations = allStations.OrderBy(x => x, distanceComparer).Take(count);

            List<NREStationLocationRecord> stationLocations = filteredStations.Select(x => new NREStationLocationRecord {
                Latitude = x.Latitude,
                Longitude = x.Longitude,
                CrsCode = x.CrsCode,
                StationName = x.StationName,
                Distance = inputCoordinates.GetDistanceTo(new GeoCoordinatePortable.GeoCoordinate(x.Latitude, x.Longitude))

            }).ToList();

            return numresults > 0 ? stationLocations.Take(numresults).ToList() : stationLocations;
        }
    }
}
