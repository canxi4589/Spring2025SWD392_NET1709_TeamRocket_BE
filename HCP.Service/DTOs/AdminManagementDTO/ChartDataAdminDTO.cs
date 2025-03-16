using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HCP.Service.DTOs.AdminManagementDTO
{
    public class ChartDataAdminDTO
    {
        public class ChartDataAdminShowDTO
        {
            public double revenue { get; set; }
            public string name { get; set; }
        }
        public class ChartDataAdminShowListDTO
        {
            public List<ChartDataAdminShowDTO> ChartData { get; set; }
        }
    }
}
