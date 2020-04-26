using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Huxley.Utils {
    public interface IStationConfig {

        Task<IList<CrsRecord>> GetCrsCodes(
            string embeddedCrsPath,
            string nreStationsUrl,
            string naptanStationsUrl,
            string stationsLog);

        Task<IList<NREStationRecord>> GetNREStations(
           string naptanStationsUrl);
    }
}
