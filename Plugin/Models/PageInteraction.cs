using System;

namespace Avalanche.Models
{
    public class PageInteraction
    {
        public DateTime InteractionTime { get; set; }
        public int PageId { get; set; }
        public string Parameter { get; set; }
    }
}
