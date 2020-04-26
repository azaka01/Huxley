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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Formo;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Huxley.Utils;
using Huxley.Models;

namespace Huxley {
    public class HuxleyApi : HttpApplication {

        // Singleton to store the station name to CRS lookup
        public static IList<CrsRecord> CrsCodes { get; private set; }

        public static NREStationModel NreStationModel = new NREStationModel();

        // Singleton to store the London Terminals CRS lookup
        public static IList<CrsRecord> LondonTerminals { get; private set; }

        // Singleton to store the Huxley settings
        public static HuxleySettings Settings { get; private set; }

        protected void Application_Start() {
            // Makes the JSON easier to read in a browser without installing an extension like JSONview
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;

            // Stops the backing field names being used instead of the public property names (*Field & PropertyChanged etc.)
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            // Returns JSON to the browser without needing to add application/json to the accept request header - remove to use XML (becomes the default)
            GlobalConfiguration.Configuration.Formatters.Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);

            // Pass Register into Configure to support attribute routing in the future
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // Load settings
            var config = new Configuration();
            Settings = config.Bind<HuxleySettings>();
            var stationConfig = new StationConfig();

            CrsCodes = stationConfig.GetCrsCodes(
                        Server.MapPath(Settings.StationLocalRef),
                        Settings.NreStationsUrl,
                        Settings.NaptanStationsUrl,
                        Server.MapPath(Settings.StationsChangeLog)).Result;

            // https://en.wikipedia.org/wiki/London_station_group 
            // Farringdon [ZFD] is not a London Terminal but it probably should be (maybe when Crossrail opens it will be)
            LondonTerminals = new List<CrsRecord> {
                new CrsRecord {CrsCode = "BFR", StationName = "London Blackfriars",},
                new CrsRecord {CrsCode = "CST", StationName = "London Cannon Street",},
                new CrsRecord {CrsCode = "CHX", StationName = "London Charing Cross",},
                new CrsRecord {CrsCode = "CTK", StationName = "City Thameslink",},
                new CrsRecord {CrsCode = "EUS", StationName = "London Euston",},
                new CrsRecord {CrsCode = "FST", StationName = "London Fenchurch Street",},
                new CrsRecord {CrsCode = "KGX", StationName = "London Kings Cross",},
                new CrsRecord {CrsCode = "LST", StationName = "London Liverpool Street",},
                new CrsRecord {CrsCode = "LBG", StationName = "London Bridge",},
                new CrsRecord {CrsCode = "MYB", StationName = "London Marylebone",},
                new CrsRecord {CrsCode = "MOG", StationName = "Moorgate",},
                new CrsRecord {CrsCode = "OLD", StationName = "Old Street",},
                new CrsRecord {CrsCode = "PAD", StationName = "London Paddington",},
                new CrsRecord {CrsCode = "STP", StationName = "London St Pancras International",},
                new CrsRecord {CrsCode = "VXH", StationName = "Vauxhall",},
                new CrsRecord {CrsCode = "VIC", StationName = "London Victoria",},
                new CrsRecord {CrsCode = "WAT", StationName = "London Waterloo",},
                new CrsRecord {CrsCode = "WAE", StationName = "London Waterloo East",},
            };

            NreStationModel.AllStations = stationConfig.GetNREStations(Settings.NaptanStationsUrl).Result.OrderBy(f => f.StationName).ToList();

            NreStationModel.InitialiseLondonStations(LondonTerminals, NreStationModel.AllStations);
        }

        protected void Application_BeginRequest(object sender, EventArgs e) {
            var application = sender as HttpApplication;
            if (application != null && application.Context != null) {
                application.Context.Response.Headers.Remove("Server");
            }
        }         
    }
}