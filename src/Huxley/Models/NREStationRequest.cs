using System;
namespace Huxley.Models {
    public class NREStationRequest
    {
        public string filter { get; set; }

        public string location { get; set; }

        public int numresults { get; set; }
    }
}
