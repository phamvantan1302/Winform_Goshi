using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformGoshi.Models.dashboard
{
    public class DMPlan
    {
        public int ngay {  get; set; }
        public double gio { get; set; }
        public double tyle { get; set; }
    }

    public class DMOrder
    {
        public int id { get; set; }
        public double plannedquantity { get; set; }
        public DateTime fromDate { get; set; }
        public DateTime toDate { get; set; }
        public int shift_id { get; set; }
    }

    public class dataViewTable
    {
        public double slplan { get; set; }
        public double slact { get; set; }
        public double sldiff { get; set; }
        public double tylecc { get; set; }
        public string tyleht { get; set; }
        public string calv { get; set; }
        public int ngay { get; set; }
    }
}
