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
        public int shift_id { get; set; }
    }

    public class dataViewTable
    {
        public double slplan { get; set; }
        public double slact { get; set; }
        public double sldiff { get; set; }
        public double tgkehoach { get; set; }
        public double tgiandung { get; set; }
        public double tgianTT { get; set; }
        public double chechlech { get; set; }
        public double tyleht { get; set; }
        public int ngay { get; set; }
    }
}
