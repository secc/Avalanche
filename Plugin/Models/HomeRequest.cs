using System.Collections.Generic;

namespace Avalanche.Models
{
    public class HomeRequest
    {
        public MobilePage Header { get; set; }
        public MobilePage Footer { get; set; }
        public MobilePage Page { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
    }
}
