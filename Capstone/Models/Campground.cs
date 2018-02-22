using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Capstone.Models
{
    public class Campground
    {
        public string Name { get; set; }
        public int OpenFromDate { get; set; }
        public int OpenToDate { get; set; }
        public decimal DailyFee { get; set; }
    }
}
