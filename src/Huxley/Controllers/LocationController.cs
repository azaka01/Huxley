using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Huxley.Models;
using GeoUK.Coordinates;
using System.IO;
using System.Web;
using System.Net.Http;
using System.Web.Http.Results;

namespace Huxley.Controllers {
    public class LocationController : ApiController {

      
        // GET /locations/{filter}/{location}/{numresults}
        public IEnumerable<NREStationRecord> Get([FromUri] NREStationRequest request) {

            // 1. start with all stations

            // 2. if there's a special filter (London Terminals) then apply it
            //    otherwise apply filter if present

            // 3. if location present then use it

            // 4. apply num results if present

            // 5. if no optional parameters then return with full data set

            var results = HuxleyApi.NreStationModel.AllStations;
            if (!string.IsNullOrWhiteSpace(request.filter)) {
                if (request.filter.Equals("London Terminals", StringComparison.InvariantCultureIgnoreCase)) {
                    results = HuxleyApi.NreStationModel.LondonStations;
                } else {
                    results = HuxleyApi.NreStationModel.AllStations.Where(c => c.StationName.IndexOf(request.filter, StringComparison.InvariantCultureIgnoreCase) >= 0).ToList();
                }
            }
                
            if (!string.IsNullOrWhiteSpace(request.location)) {
                var latlon = request.location.Split(',').ToArray();
                LatitudeLongitude l = new LatitudeLongitude(Double.Parse(latlon[0]), Double.Parse(latlon[1]));

                var v = new GeoCoordinatePortable.GeoCoordinate(l.Latitude, l.Longitude);

                var comparitor = new DistanceComparer();
                comparitor.TargetLocation = v;
                var count = results.Count();
                if (request.numresults > 0) {
                    count = request.numresults;
                }

                var filteredStations = results.OrderBy(x => x, comparitor).Take(count);

                List<NREStationLocationRecord> stationLocations = filteredStations.Select(x => new NREStationLocationRecord {
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    CrsCode = x.CrsCode,
                    StationName = x.StationName,
                    Distance = v.GetDistanceTo(new GeoCoordinatePortable.GeoCoordinate(x.Latitude, x.Longitude))

                }).ToList();

                if (request.numresults > 0)
                    return stationLocations.Take(request.numresults);
                else
                    return stationLocations;
            }

            if (request.numresults > 0)
                return results.Take(request.numresults);
            else
                return results;
        }
    }
}
