using System.Collections.Generic;

namespace SomeWeirdApplicationBackend.Models
{
    public class PointData
    {
        public string Name { get; set; }

        public int TurboThreshold { get; set; }

        public IEnumerable<object >Data { get; set; }
    }
}
