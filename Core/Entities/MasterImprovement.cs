using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class MasterImprovement
    {
        public int Id { get; set; }

        public string? ParentPartNumber { get; set; } = null!;

        public int Line { get; set; }

        public decimal? OldCycleTime { get; set; }

        public decimal? NewCycleTime { get; set; }

        public DateTime ImprovementDate { get; set; }

        public string? Process { get; set; }

        public string? Description { get; set; }

        public decimal? TimeSaved => OldCycleTime - NewCycleTime;
    }
}
