using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Repository.Entities
{
    public class ServiceTimeSlot : BaseEntity
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DateTime? DateStart { get; set; }
        public string DayOfWeek { get; set; }
        public bool IsBook { get; set; }
        public string Status { get; set; }
        [ForeignKey("Service")]
        public Guid ServiceId { get; set; }
        public CleaningService Service { get; set; }
    }
}
