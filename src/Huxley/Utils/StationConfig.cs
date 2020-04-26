using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CsvHelper;
using GeoUK;
using GeoUK.Coordinates;
using GeoUK.Ellipsoids;
using GeoUK.Projections;

namespace Huxley.Utils {
    public class StationConfig : IStationConfig {

        public async Task<IList<NREStationRecord>> GetNREStations(string naptanStationsUrl) {

            var stations = new List<NREStationRecord>();

            using (var client = new HttpClient()) {
                var stream = await client.GetStreamAsync(naptanStationsUrl);
                using (var csvReader = new CsvReader(new StreamReader(stream))) {
                    AddCodesToList(stations, csvReader);
                }
            }
            return stations;
        }

        public async Task<IList<CrsRecord>> GetCrsCodes(
            string embeddedCrsPath,
            string nreStationsUrl,
            string naptanStationsUrl,
            string stationsLog) {
            var codes = new List<CrsRecord>();

            // NRE list - incomplete / old (some codes only in NaPTAN work against the Darwin web service)

            try {
                using (var client = new HttpClient()) {
                    var stream = await client.GetStreamAsync(nreStationsUrl);
                    using (var csvReader = new CsvReader(new StreamReader(stream))) {
                        // Need a custom map as NRE headers are different to NaPTAN
                        csvReader.Configuration.RegisterClassMap<NreCrsRecordMap>();
                        AddCodesToList(codes, csvReader);
                    }
                }
                // ReSharper disable EmptyGeneralCatchClause
            } catch {
                // Don't do anything if this fails as we try to load from NaPTAN next
                // ReSharper restore EmptyGeneralCatchClause
            }

            try {
                // First try to get the latest version
                using (var client = new HttpClient()) {
                    var stream = await client.GetStreamAsync(naptanStationsUrl);
                    using (var csvReader = new CsvReader(new StreamReader(stream))) {
                        AddCodesToList(codes, csvReader);
                    }
                }
            } catch {
                try {
                    // If we can't get the latest version then use the embedded version
                    // Might be a little bit out of date but probably good enough
                    using (var stream = File.OpenRead(embeddedCrsPath)) {
                        using (var csvReader = new CsvReader(new StreamReader(stream))) {
                            AddCodesToList(codes, csvReader);
                        }
                    }
                    // ReSharper disable EmptyGeneralCatchClause
                } catch {
                    // If this doesn't work continue to start up
                    // ReSharper restore EmptyGeneralCatchClause
                }
            }

            return codes;
        }

        private void AddCodesToList(List<CrsRecord> codes, CsvReader csvReader) {
            // Enumerate results and add to a list as reader can only be enumerated once
            // Only missing codes are added to the list (first pass will add all codes)
            codes.AddRange(csvReader.GetRecords<CrsRecord>().Where(c => codes.All(code => code.CrsCode != c.CrsCode))
                                    .Select(c => new CrsRecord {
                                    // NaPTAN suffixes most station names with "Rail Station" which we don't want
                                    StationName = c.StationName.Replace("Rail Station", "").Trim(),
                                        CrsCode = c.CrsCode
                                    }));
        }

        private void AddCodesToList(List<NREStationRecord> codes, CsvReader csvReader)
        {
            // Enumerate results and add to a list as reader can only be enumerated once
            // Only missing codes are added to the list (first pass will add all codes)
            codes.AddRange(csvReader.GetRecords<NaptaStationRecord>().Where(c => codes.All(code => code.CrsCode != c.CrsCode))
                                    .Select(c => fromNaptaStation(c)));

        }

        private NREStationRecord fromNaptaStation(NaptaStationRecord naptaStation)
        {
            var v = GetLatLon(Double.Parse(naptaStation.Easting), Double.Parse(naptaStation.Northing));
            return new NREStationRecord {
                // NaPTAN suffixes most station names with "Rail Station" which we don't want
                StationName = naptaStation.StationName.Replace("Rail Station", "").Trim(),
                CrsCode = naptaStation.CrsCode,
                Latitude = v.Latitude,
                Longitude = v.Longitude
            };
        }

        private LatitudeLongitude GetLatLon(double easting, double northing) {
            var cartesian = GeoUK.Convert.ToCartesian(new Airy1830(),
                                                      new BritishNationalGrid(),
                                                      new EastingNorthing(
                                                            easting,
                                                            northing));
            var wgsCartesian = Transform.Osgb36ToEtrs89(cartesian);
            return GeoUK.Convert.ToLatitudeLongitude(new Wgs84(), wgsCartesian);
        }
    }
}