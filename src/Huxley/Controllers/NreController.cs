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
    public class NreController : ApiController {
        // GET /crslocations/{filter}/{location}/{numresults}
        [HttpGet]
        public IHttpActionResult Get([FromUri] NREStationRequest request) {

            DateTime lastWriteTime = File.GetLastWriteTime(
              HttpContext.Current.Server.MapPath(HuxleyApi.Settings.StationsChangeLog));

            var header = Request.Headers.IfModifiedSince;

            if (header != null) {
                var modifiedTime = DateTime.Parse(header.Value.ToString()).ToUniversalTime();
                var Response304 = Request.CreateResponse(System.Net.HttpStatusCode.NotModified);
                //return ResponseMessage(Response304);


                return StatusCode(System.Net.HttpStatusCode.NotModified);
               // return new ResponseMessageResult(Response304);
                // return StatusCode(System.Net.HttpStatusCode.NotModified);
            }

            var response = Request.CreateResponse(System.Net.HttpStatusCode.OK, HuxleyApi.NreStationModel.LondonStations);

          //  var response = Request.CreateResponse(System.Net.HttpStatusCode.NotModified, HuxleyApi.NreStationModel.LondonStations);

            response.Content.Headers.Add("Last-Modified", lastWriteTime.ToUniversalTime().ToString("R"));


            //    response.Headers.Add("Last-Modified", System.DateTime.UtcNow.ToString());

            return ResponseMessage(response);
        }

      /*  public IEnumerable<NREStationRecord> Get([FromUri] NREStationRequest request) {

          
            if (string.IsNullOrWhiteSpace(request.filter))
                return HuxleyApi.NreStationModel.AllStations;

            if (request.filter.Equals("London Terminals", StringComparison.InvariantCultureIgnoreCase)) {
                return HuxleyApi.NreStationModel.LondonStations;
            }

            if (!string.IsNullOrWhiteSpace(request.location)) {
                var latlon = request.location.Split(',').ToArray();
                LatitudeLongitude l = new LatitudeLongitude(Double.Parse(latlon[0]), Double.Parse(latlon[1]));

                var v = new GeoCoordinatePortable.GeoCoordinate(l.Latitude, l.Longitude);

                var comparitor = new DistanceComparer();
                comparitor.TargetLocation = v;
                var count = HuxleyApi.NreStationModel.AllStations.Count();
                if (request.numresults > 0) {
                    count = request.numresults;
                }

                var allStations = HuxleyApi.NreStationModel.AllStations.OrderBy(x => x, comparitor).Take(count);

                List<NREStationLocationRecord> allStationLocations = allStations.Select(x => new NREStationLocationRecord {
                    Latitude = x.Latitude,
                    Longitude = x.Longitude,
                    CrsCode = x.CrsCode,
                    StationName = x.StationName,
                    Distance = v.GetDistanceTo(new GeoCoordinatePortable.GeoCoordinate(x.Latitude, x.Longitude))
                    
                }).ToList();
              
                return allStationLocations;
            }
           
            // Could use a RegEx here but putting user input into a RegEx can be dangerous
            var results = HuxleyApi.NreStationModel.AllStations.Where(c => c.StationName.IndexOf(request.filter, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return results;
        }*/
    }
}
