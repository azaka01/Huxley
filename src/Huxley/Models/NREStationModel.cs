using System;
using System.Collections.Generic;
using System.Linq;

namespace Huxley.Models {
    public class NREStationModel {
        public IList<NREStationRecord> AllStations { get; set; }
        public IList<NREStationRecord> LondonStations { get; set; }
        public void InitialiseLondonStations
            (IList<CrsRecord> crsRecords,
            IList<NREStationRecord> nreRecords)
        {
            var aCodes = new HashSet<String>(crsRecords.Select(a => a.CrsCode));
            LondonStations = nreRecords.Where(b => aCodes.Contains(b.CrsCode)).ToList();
        }
     }
}