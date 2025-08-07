using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformGoshi.Models.dashboard
{
    public class DashboardTTSX
    {
        public class DMTimeByCa
        {
            public int id { get; set; }
            public string name { get; set; }
            public string mondayhours { get; set; }
            public string tuesdayhours { get; set; }
            public string wednesdayhours { get; set; }
            public string thursdayhours { get; set; }
            public string fridayhours { get; set; }
            public string saturdayhours { get; set; }
            public string sundayhours { get; set; }
            public string namesc {  get; set; }
            public DateTime fromdate { get; set; }
            public DateTime todate { get; set; }
            public string type {  get; set; }
        }

        public class DMShowChart
        {
            public string model { get; set; }
            public string status { get; set; }
            public string start { get; set; }
            public string end { get; set; }
            public double slstart { get; set; } = 0;
            public double slend { get; set; } = 0;
            public double slsx {  get; set; }
        }

        public class DMChuyen 
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class DMModel
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class DMShift
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class DMCounterinfo
        {
            public int id { get; set; }
            public string machinecode { get; set; }
            public double quantity { get; set; }
            public double counter { get; set; }
            public DateTime created_at { get; set; }
        }
        public class DMStatusinfo
        {
            public int id { get; set; }
            public string machinecode { get; set; }
            public string status { get; set; }
            public DateTime created_at { get; set; }
            public string error_code { get; set; }
            public int stoppagereason_id { get; set; }
            public int shifttimetableexception_id { get; set; }
            public string stop_name { get; set; }
            public string shifttimetableexception_name { get; set; }
            public DateTime fromdate { get; set; }
            public DateTime todate { get; set; }
        }
    }
}
