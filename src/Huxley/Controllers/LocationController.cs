using System.Collections.Generic;
using System.Web.Http;
using Huxley.Models;
using Huxley.Utils;
using System.Net;

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
           
            if (!ModelState.IsValid) {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            var results = request.GetFilteredResults();
           
            if (request.HasLocation()) {

                var locationRequestHandler = new LocationRequestHander();

                return locationRequestHandler.GetNearestStations(request.Location, results, request.Numresults);
            }

            return request.GetResults(results);
        }
    }
}
