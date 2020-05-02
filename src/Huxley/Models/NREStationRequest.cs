using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Huxley.Models {
    public class NREStationRequest
    {
        public string Filter { get; set; }

        public string Location { get; set; }

        [Range(0, 10000)]
        public int Numresults { get; set; }

        public bool HasFilter() {
            return !string.IsNullOrWhiteSpace(Filter);
        }

        public bool HasLocation() {
            return !string.IsNullOrWhiteSpace(Location);
        }

        public bool IsLondonStationsFilter() {
            return Filter.Equals("London Terminals", StringComparison.InvariantCultureIgnoreCase);
        }

        public IList<NREStationRecord> GetFilteredResults() {
            if (HasFilter()) {
                if (IsLondonStationsFilter()) {
                    return HuxleyApi.NreStationModel.LondonStations;
                } else {
                    return HuxleyApi.NreStationModel.AllStations.Where(c => c.StationName.IndexOf(Filter, StringComparison.InvariantCultureIgnoreCase) >= 0).ToList();
                }
            }

            return HuxleyApi.NreStationModel.AllStations;
        }

        public IEnumerable<NREStationRecord> GetResults(IList<NREStationRecord> stationRecords) {

            return Numresults > 0 ? stationRecords.Take(Numresults) : stationRecords;

        }
    }
}
