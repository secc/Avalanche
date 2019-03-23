using System.Collections.Generic;

namespace Avalanche.Models
{
    public class PersonSummary
    {
        public string NickName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Campus { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        public List<GroupSummary> Groups { get; set; }
    }
}
