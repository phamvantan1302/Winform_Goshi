using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using WinformGoshi.Models.dashboard;
using WinformGoshi.Sevices.dashboard;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static WinformGoshi.Models.dashboard.DashboardTTSX;

namespace WinformGoshi.Forms.Dashboard
{
    public partial class frmDashboardTTSX : Form
    {
        private System.Windows.Forms.Timer tRefreshData;
        private System.Windows.Forms.Timer tRefreshDataD;
        private bool isFormClosing = false;
        private bool checktb = true;
        private DateTime datenow = DateTime.Now;
        public frmDashboardTTSX()        
        {
            InitializeComponent();

            List<DMChuyen> lsChuyen = frmDashboardTTSXServices.getDSChuyen(datenow);
            if (lsChuyen.Count > 0)
            {
                cbbChuyen.ValueMember = "id";
                cbbChuyen.DisplayMember = "name";
                cbbChuyen.DataSource = lsChuyen;
                cbbChuyen.SelectedIndex = 0;

                List<DMModel> lsmodel = frmDashboardTTSXServices.getDSModel(datenow, cbbChuyen.SelectedValue.ToString());
                if (lsmodel.Count > 0)
                {
                    cbbModel.ValueMember = "id";
                    cbbModel.DisplayMember = "name";
                    cbbModel.DataSource = lsmodel;
                    cbbModel.SelectedIndex = 0;
                }

                List<DMShift> lsca = frmDashboardTTSXServices.getIdCa(datenow, cbbChuyen.SelectedValue.ToString());
                if (lsca.Count > 0)
                {
                    cbbCa.ValueMember = "id";
                    cbbCa.DisplayMember = "name";
                    cbbCa.DataSource = lsca;
                    cbbCa.SelectedIndex = 0;
                }
            }
            else
            {
                cbbChuyen.DataSource = null;
            }
            loaddata(null, null);
            tRefreshData = new System.Windows.Forms.Timer();
            tRefreshData.Tick += new EventHandler(loaddata);
            tRefreshData.Interval = 15000;
            tRefreshData.Enabled = true;

            // load màu đèn
            loadden(null, null);
            tRefreshDataD = new System.Windows.Forms.Timer();
            tRefreshDataD.Tick += new EventHandler(loadden);
            tRefreshDataD.Interval = 500;
            tRefreshDataD.Enabled = true;
        }

        private void clearform()
        {
            lbPlan.Text = "0";
            lbACT.Text = "0";
            lbDIF.Text = "0";
            lbTotalPlan.Text = "0";
            lbTotalACT.Text = "0";
            lbTotalDIF.Text = "0";
        }

        private void loadden(object sender, EventArgs e)
        {
            string line = "0", mode = "0", mmtb = "", status = "";
            int idca = 0;

            if (cbbChuyen.SelectedValue != null)
                line = cbbChuyen.SelectedValue.ToString();
            if (cbbModel.SelectedValue != null)
                mode = cbbModel.SelectedValue.ToString();
            if (cbbCa.SelectedValue != null)
                idca = int.Parse(cbbCa.SelectedValue.ToString());

            mmtb = frmDashboardTTSXServices.getMMTB(datenow, line, mode);

            status = frmDashboardTTSXServices.getStatusinfo(mmtb);
            btnMD.BackColor = Color.White;
            btnRun.BackColor = Color.White;
            btnStop.BackColor = Color.White;
            if (checktb)
            {
                checktb = false;
            }
            else
            {
                switch (status)
                {
                    case "MAY_CHAY":
                        btnRun.BackColor = Color.Green;
                        break;
                    case "MAY_LOI":
                        btnStop.BackColor = Color.Red;
                        break;
                    default:
                        btnMD.BackColor = Color.Yellow;
                        break;
                }
                checktb = true;
            }
        }

        private void loaddata(object sender, EventArgs e)
        {
            if (isFormClosing || chart1 == null || this.IsDisposed)
                return;

            clearform();
            string valueThu = "", bdau = "", kthuc = "", line = "0", mode = "0", mmtb = "";
            int idca = 0;
            DateTime timebd = DateTime.Now, timekt = DateTime.Now;
            double sumtime = 0, sumtimeact = 0, timestart = 0, timeend = 0, slsxkh = 0, timedung = 0, timeCT = 0, timedungByPhut = 0;

            if (cbbChuyen.SelectedValue != null)
                line = cbbChuyen.SelectedValue.ToString();
            if(cbbModel.SelectedValue != null)
                mode = cbbModel.SelectedValue.ToString();
            if (cbbCa.SelectedValue != null)
                idca = int.Parse(cbbCa.SelectedValue.ToString());

            mmtb = frmDashboardTTSXServices.getMMTB(datenow, line, mode);
            List<DMTimeByCa> getTimeCa = frmDashboardTTSXServices.getDataCaKH(datenow, idca);
            double slkh = frmDashboardTTSXServices.getSLKH(datenow, line, idca);
            List<DMShowChart> dschart = new List<DMShowChart>();
            List<DMShowChart> dschartError = new List<DMShowChart>();
            DMShowChart dMShowChart = null;
            DMShowChart dMShowChartError = null;
            chart1.ChartAreas[0].AxisX.StripLines.Clear();

            if (getTimeCa.Count > 0)
            {
                //get tgian lam viec theo KH
                foreach (var it in getTimeCa)
                {
                    timedung = (it.todate - it.fromdate).TotalSeconds + timedung;
                }

                DateTime dt = getTimeCa.FirstOrDefault().fromdate;
                string thu = dt.DayOfWeek.ToString().ToLower() + "hours";

                var item = getTimeCa.FirstOrDefault();

                PropertyInfo prop = typeof(DMTimeByCa).GetProperty(thu, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (prop != null)
                {
                    valueThu = prop.GetValue(item)?.ToString();
                }

                if (valueThu != "")
                {
                    bdau = valueThu.Split('-')[0];
                    kthuc = valueThu.Split('-')[1];

                    DateTime bdTime, ktTime;

                    if (DateTime.TryParse(bdau, out bdTime) && DateTime.TryParse(kthuc, out ktTime))
                    {
                        bdau = bdTime.ToString("HH:mm");
                        kthuc = ktTime.ToString("HH:mm");

                        timekt = new DateTime(
                        datenow.Year,
                        datenow.Month,
                        datenow.Day,
                        ktTime.Hour,
                        ktTime.Minute,
                        0
                        );

                        if (ktTime < bdTime) 
                        {
                            var preDay = datenow.AddDays(-1);
                            timebd = new DateTime(
                                preDay.Year,
                                preDay.Month,
                                preDay.Day,
                                bdTime.Hour,
                                bdTime.Minute,
                                0
                                );
                        }
                        else
                        {
                            timebd = new DateTime(
                                datenow.Year,
                                datenow.Month,
                                datenow.Day,
                                bdTime.Hour,
                                bdTime.Minute,
                                0
                            );
                        }
                    }

                    DateTime datefix = new DateTime(2025, 8, 6, 18, 40, 0);
                    DateTime datetamthoi = new DateTime(2025, 8, 6, 18, 38, 0);
                    if (timebd == datefix)
                    {
                        timebd = datetamthoi;
                    }

                    double sumtimeca = (timekt - timebd).TotalSeconds;
                    timeCT = (sumtimeca - timedung) / slkh;

                    // Bdau ca
                    dMShowChart = new DMShowChart()
                    {
                        model = "",
                        status = "startup",
                        start = timebd.ToString("HH:mm"),
                        end = item.fromdate.ToString("HH:mm"),
                        slstart = 0,
                        slend = 0
                    };
                    dschart.Add(dMShowChart);
                    // Tam dung
                    for(int i = 0; i< getTimeCa.Count; i++)
                    {
                        dMShowChart = new DMShowChart()
                        {
                            model = "",
                            status = getTimeCa[i].namesc,
                            start = getTimeCa[i].fromdate.ToString("HH:mm"),
                            end = getTimeCa[i].todate.ToString("HH:mm"),
                        };
                        if(i - 1 >= 0)
                            dMShowChart.slstart = Math.Round(sumtime/timeCT, 0);
                        else
                            dMShowChart.slstart = 0;
                        dMShowChart.slend = dMShowChart.slstart;
                        dschart.Add(dMShowChart);

                        //máy chay sau khi dung
                        if(dMShowChart.end != kthuc)
                        {
                            dMShowChart = new DMShowChart()
                            {
                                model = "",
                                status = "Máy chạy",
                                start = getTimeCa[i].todate.ToString("HH:mm"),
                                slstart = dschart.LastOrDefault().slend
                            };

                            if (i + 1 < getTimeCa.Count)
                            {
                                sumtime = (getTimeCa[i + 1].fromdate - getTimeCa[i].todate).TotalSeconds + sumtime;
                                dMShowChart.end = getTimeCa[i + 1].fromdate.ToString("HH:mm");
                                dMShowChart.slend = Math.Round(sumtime / timeCT, 0);
                            }
                            else
                            {
                                sumtime = (timekt - getTimeCa[i].todate).TotalSeconds + sumtime;
                                dMShowChart.end = timekt.ToString("HH:mm");
                                dMShowChart.slend = slkh;
                            }
                                
                            dschart.Add(dMShowChart);
                        }

                    }
                    // Kthuc ca
                    dMShowChart = new DMShowChart()
                    {
                        model = "",
                        status = "close",
                        start = timekt.ToString("HH:mm"),
                        end = timekt.ToString("HH:mm")
                    };
                    dMShowChart.slstart = slkh;
                    dMShowChart.slend = slkh;
                    dschart.Add(dMShowChart);
                }
            }

            // show PLAN
            Series planSeries = chart1.Series["PLAN"];
            planSeries.ChartType = SeriesChartType.Line;
            //chart1.Series["PLAN"].IsValueShownAsLabel = true;
            planSeries.Points.Clear();

            DateTime today = datenow;

            DateTime? prevTime = null;
            DateTime curDay = today;
            List<DateTime> timePoints = new List<DateTime>();
            bool checkdate = false;

            foreach (var item in dschart.Where(x => x.start != x.end))
            {
                var tsStart = TimeSpan.Parse(item.start);
                var tsEnd = TimeSpan.Parse(item.end);
                var tscheck = TimeSpan.Parse(timekt.ToString("HH:mm:ss"));

                DateTime start = curDay.Date.Add(tsStart);
                DateTime end = curDay.Date.Add(tsEnd);

                // Nếu end < tscheck → tức là qua ngày
                if (tsEnd > tscheck)
                {
                    end = curDay.AddDays(-1).Date.Add(tsEnd);
                }
                else
                {
                    end = curDay.Date.Add(tsEnd);
                }
                if (tsStart > tscheck)
                {
                    start = curDay.AddDays(-1).Date.Add(tsStart);
                }
                else
                {
                    start = curDay.Date.Add(tsStart);
                }

                timePoints.Add(start);
                timePoints.Add(end);
                prevTime = end;
            }

            timePoints = timePoints.Distinct().OrderBy(t => t).ToList();


            foreach (var item in dschart)
            {
                if (item.start == item.end) continue; // Bỏ khoảng không hợp lệ

                var tsStart = TimeSpan.Parse(item.start);
                var tsEnd = TimeSpan.Parse(item.end);
                var tscheck = TimeSpan.Parse("18:01:00");

                DateTime start = curDay.Date.Add(tsStart);
                DateTime end = curDay.Date.Add(tsEnd);

                // Nếu end < start → tức là qua ngày
                if (tsEnd >= tscheck)
                {
                    end = curDay.AddDays(-1).Date.Add(tsEnd);
                }
                else
                {
                    end = curDay.Date.Add(tsEnd);
                }
                if (tsStart >= tscheck)
                {
                    start = curDay.AddDays(-1).Date.Add(tsStart);
                }
                else
                {
                    start = curDay.Date.Add(tsStart);
                }

                planSeries.Points.AddXY(start.ToOADate(), item.slstart);
                planSeries.Points.AddXY(end.ToOADate(), item.slend);

                var dpEnd = new DataPoint(end.ToOADate(), item.slend);

                // Gán label chỉ cho điểm kết thúc
                dpEnd.Label = item.slend.ToString();
                planSeries.Points.Add(dpEnd);
            }

            // data ACT+SuCo
            List<DMStatusinfo> lsstatus1 = frmDashboardTTSXServices.getDSStatusinfo(timebd, timekt, mmtb);
            // lọc trang thái dừng và lỗi đầu vào
            DMStatusinfo dmstatus = null;
            List<DMStatusinfo> lsstatus = new List<DMStatusinfo>();

            //filter TT
            try
            {
                if (lsstatus1.Count > 0)
                {
                    for (int i = 0; i < lsstatus1.Count; i++)
                    {
                        var current = lsstatus1[i];
                        var next = lsstatus1[i];
                        var nextMC = lsstatus1[i];
                        if (i != lsstatus1.Count - 1)
                        {
                            next = lsstatus1[i + 1];
                            nextMC = lsstatus1[i + 1];
                        }

                        // Trường hợp MAY_DUNG kéo dài hơn 5 phút
                        if (current.status == "MAY_DUNG" || current.status == "KHONG_CO_TRANG_THAI")
                        {
                            var duration = (nextMC.created_at - current.created_at).TotalSeconds;

                            if (duration == 0 && DateTime.Now < timekt)
                            {
                                duration = (DateTime.Now - current.created_at).TotalSeconds;
                            }
                            else if(duration == 0 && DateTime.Now >= timekt)
                            {
                                duration = (timekt - current.created_at).TotalSeconds;
                            }

                            if (duration > 300)
                            {
                                // Thêm trạng thái MAY_DUNG
                                lsstatus.Add(CloneStatus(current));

                                // Nếu trạng thái tiếp theo là MAY_CHAY thì thêm nó để đánh dấu kết thúc
                                if (next.status == "MAY_CHAY")
                                {
                                    lsstatus.Add(CloneStatus(next));
                                }
                                // Bỏ qua các trạng thái MAY_DUNG liên tiếp tiếp theo
                                while (i + 1 < lsstatus1.Count && (lsstatus1[i + 1].status == "MAY_DUNG" || lsstatus1[i + 1].status == "KHONG_CO_TRANG_THAI"))
                                {
                                    i++;
                                }
                                // Thêm máy chạy **sau khi kết thúc dừng** nếu có
                                if (i + 1 < lsstatus1.Count && (lsstatus1[i + 1].status != "MAY_DUNG" || lsstatus1[i + 1].status != "KHONG_CO_TRANG_THAI"))
                                {
                                    // KIểm tra nếu máy lỗi < 120 thì next đến máy chạy nếu > 120 thì thoát
                                    if (lsstatus1[i + 1].status == "MAY_LOI")
                                    {
                                        current = lsstatus1[i + 1];
                                        if (i != lsstatus1.Count - 1)
                                        {
                                            for (int j = i + 1; j < lsstatus1.Count; j++)
                                            {
                                                if (lsstatus1[j].status != "MAY_LOI")
                                                {
                                                    nextMC = lsstatus1[j];
                                                    break;
                                                }
                                            }
                                        }
                                        duration = (nextMC.created_at - current.created_at).TotalSeconds;

                                        if (duration == 0 && DateTime.Now < timekt)
                                        {
                                            duration = (DateTime.Now - current.created_at).TotalSeconds;
                                        }
                                        if (duration < 120)
                                        {
                                            for (int j = i + 1; j < lsstatus1.Count; j++)
                                            {
                                                if (lsstatus1[j].status == "MAY_CHAY")
                                                {
                                                    lsstatus.Add(CloneStatus(lsstatus1[j]));
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                        lsstatus.Add(CloneStatus(lsstatus1[i + 1]));
                                }
                                if (lsstatus.Count > 1)
                                {
                                    timedungByPhut += (lsstatus.LastOrDefault().created_at - lsstatus[lsstatus.Count - 2].created_at).TotalMinutes;
                                }
                            }
                        }
                        // Trường hợp MAY_LOI
                        else if (current.status == "MAY_LOI")
                        {
                            int countttnext = 0;
                            if (i != lsstatus1.Count - 1)
                            {
                                for (int j = i + 1; j < lsstatus1.Count; j++)
                                {
                                    countttnext = j;
                                    //if (lsstatus1[j].status != "MAY_LOI")
                                    //{
                                        nextMC = lsstatus1[j];
                                        break;
                                    //}
                                    //else
                                    //{
                                    //    nextMC = lsstatus1[j];
                                    //    //current = lsstatus1[j];
                                    //    break;
                                    //}
                                }
                            }else
                                countttnext = i;

                            var duration = (nextMC.created_at - current.created_at).TotalSeconds;

                            if (duration == 0 && DateTime.Now < timekt && countttnext == lsstatus1.Count-1)
                            {
                                duration = (DateTime.Now - current.created_at).TotalSeconds;
                            }
                            else if(duration == 0 && DateTime.Now >= timekt && countttnext == lsstatus1.Count-1)
                            {
                                duration = (timekt - current.created_at).TotalSeconds;
                            }

                            if (duration >= 120)
                            {
                                lsstatus.Add(CloneStatus(current));

                                // Chỉ thêm nếu trạng thái tiếp theo là MAY_CHAY (bỏ qua MAY_DUNG)
                                //if (next.status == "MAY_CHAY")
                                //{
                                //    lsstatus.Add(CloneStatus(next));
                                //}
                                for (int j = countttnext; j < lsstatus1.Count; j++)
                                {
                                    if (lsstatus1[j].status == "MAY_CHAY")
                                    {
                                        lsstatus.Add(CloneStatus(lsstatus1[j]));
                                        break;
                                    }
                                    else if (lsstatus1[j].status == "MAY_LOI")
                                    {
                                        break;
                                    }
                                    else if (lsstatus1[j].status == "MAY_DUNG" || lsstatus1[j].status == "KHONG_CO_TRANG_THAI")
                                    {
                                        if (j != lsstatus1.Count - 1)
                                        {
                                            var nextMD = lsstatus1[j + 1];
                                            var durationd = (nextMD.created_at - lsstatus1[j].created_at).TotalSeconds;
                                            if (durationd > 300)
                                            {
                                                //lsstatus.Add(CloneStatus(lsstatus1[j]));
                                                i = j-1;
                                                break;
                                            }
                                            //else
                                            //{
                                                //while (j + 1 < lsstatus1.Count && (lsstatus1[j + 1].status == "MAY_DUNG" || lsstatus1[j + 1].status == "KHONG_CO_TRANG_THAI"))
                                                //{
                                                //    j++;
                                                //}
                                                //if (j + 1 < lsstatus1.Count && (lsstatus1[j + 1].status != "MAY_DUNG" || lsstatus1[j + 1].status != "KHONG_CO_TRANG_THAI"))
                                                //{
                                                //    if (lsstatus1[j + 1].status == "MAY_LOI")
                                                //    {
                                                //        break;
                                                //    }
                                                //    lsstatus.Add(CloneStatus(lsstatus1[j + 1]));
                                                //}
                                            //}
                                        }

                                    }

                                }
                                if (lsstatus.Count > 1)
                                {
                                    timedungByPhut += (lsstatus.LastOrDefault().created_at - lsstatus[lsstatus.Count - 2].created_at).TotalMinutes;
                                }
                            }
                            else if(lsstatus.Count > 0)
                            {
                                if (lsstatus.LastOrDefault().status != "MAY_CHAY")
                                {
                                    for (int j = i + 1; j < lsstatus1.Count; j++)
                                    {
                                        if (lsstatus1[j].status == "MAY_CHAY")
                                        {
                                            lsstatus.Add(CloneStatus(lsstatus1[j]));
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }

            List<DMCounterinfo> lscounter = frmDashboardTTSXServices.getDSCounterinfo(timebd, timekt, mmtb);
            lscounter = lscounter
                        .Where(x => x.quantity != 0)
                        .ToList();

            dschart = new List<DMShowChart>();
            dschartError = new List<DMShowChart>();
            if (lsstatus.Count > 0 && lscounter.Count > 0)
            {
                dMShowChart = new DMShowChart()
                {
                    status = "MAY_CHAY",
                    start = timebd.ToString("HH:mm"),
                    slstart = 0,
                    slend = 0
                };
                sumtimeact = 0;
                dMShowChart.end = lscounter.FirstOrDefault().created_at.ToString("HH:mm");
                dschart.Add(dMShowChart);

                dMShowChart = new DMShowChart()
                {
                    status = "",
                    start = lscounter.FirstOrDefault().created_at.ToString("HH:mm"),
                    slstart = 0,
                    slend = 0
                };
                sumtimeact = 0;
                dMShowChart.end = lsstatus.FirstOrDefault().created_at.ToString("HH:mm");
                dMShowChart.slend = lscounter.Where(x => x.created_at <= lsstatus.FirstOrDefault().created_at)
                                                        .OrderByDescending(x => x.created_at)
                                                        .FirstOrDefault()?.quantity ?? 0;
                dschart.Add(dMShowChart);

                for (int i = 0; i < lsstatus.Count; i++)
                {
                    if (lsstatus[i].status == "MAY_DUNG" || lsstatus[i].status == "KHONG_CO_TRANG_THAI")
                    {
                        if (1==1)
                        {
                            dMShowChart = new DMShowChart()
                            {
                                status = lsstatus[i].status,
                                start = lsstatus[i].created_at.ToString("HH:mm"),
                            };
                            if (i + 1 == lsstatus.Count)
                            {
                                if (DateTime.Now < timekt)
                                {
                                    dMShowChart.end = DateTime.Now.ToString("HH:mm");
                                    dMShowChart.slend = lscounter
                                                        .Where(x => x.created_at <= lsstatus[i].created_at)
                                                        .OrderByDescending(x => x.created_at)
                                                        .FirstOrDefault()?.quantity ?? 0;
                                }
                                else
                                {
                                    dMShowChart.end = timekt.ToString("HH:mm");
                                    dMShowChart.slend = lscounter
                                                        .Where(x => x.created_at <= lsstatus[i].created_at)
                                                        .OrderByDescending(x => x.created_at)
                                                        .FirstOrDefault()?.quantity ?? 0;
                                }
                                
                            }
                            else
                            {
                                dMShowChart.end = lsstatus[i + 1].created_at.ToString("HH:mm");
                                dMShowChart.slend = lscounter
                                                    .Where(x => x.created_at <= lsstatus[i].created_at)
                                                    .OrderByDescending(x => x.created_at)
                                                    .FirstOrDefault()?.quantity ?? 0;
                            }
                            dMShowChart.slstart = lscounter
                                                    .Where(x => x.created_at <= lsstatus[i].created_at)
                                                    .OrderByDescending(x => x.created_at)
                                                    .FirstOrDefault()?.quantity ?? 0;
                            dschart.Add(dMShowChart);
                        }
                        
                    }
                    else if (lsstatus[i].status == "MAY_CHAY")
                    {
                        dMShowChart = new DMShowChart()
                        {
                            status = lsstatus[i].status,
                            start = lsstatus[i].created_at.ToString("HH:mm"),
                        };
                        if (i + 1 == lsstatus.Count)
                        {
                            if (timekt < DateTime.Now)
                            {
                                dMShowChart.end = timekt.ToString("HH:mm");
                                dMShowChart.slend = lscounter
                                                    .Where(x => x.created_at <= timekt && x.quantity > 0)
                                                    .OrderByDescending(x => x.created_at)
                                                    .FirstOrDefault()?.quantity ?? 0;
                                sumtimeact = (timekt - lsstatus[i].created_at).TotalSeconds + sumtimeact;
                            }
                            else
                            {
                                dMShowChart.end = DateTime.Now.ToString("HH:mm");
                                dMShowChart.slend = lscounter
                                                    .Where(x => x.created_at <= DateTime.Now && x.quantity > 0)
                                                    .OrderByDescending(x => x.created_at)
                                                    .FirstOrDefault()?.quantity ?? 0;
                                sumtimeact = (DateTime.Now - lsstatus[i].created_at).TotalSeconds + sumtimeact;
                            }

                            
                        }
                        else
                        {
                            dMShowChart.end = lsstatus[i + 1].created_at.ToString("HH:mm");
                            dMShowChart.slend = lscounter
                                                .Where(x => x.created_at <= lsstatus[i+1].created_at && x.quantity > 0)
                                                .OrderByDescending(x => x.created_at)
                                                .FirstOrDefault()?.quantity ?? 0;
                            sumtimeact = (lsstatus[i + 1].created_at - lsstatus[i].created_at).TotalSeconds + sumtimeact;
                        }
                        dMShowChart.slstart = lscounter
                                                .Where(x => x.created_at <= lsstatus[i].created_at)
                                                .OrderByDescending(x => x.created_at)
                                                .FirstOrDefault()?.quantity ?? 0;
                        dschart.Add(dMShowChart);
                    }
                    else if (lsstatus[i].status == "MAY_LOI")
                    {
                        dMShowChart = new DMShowChart()
                        {
                            status = lsstatus[i].status,
                            start = lsstatus[i].created_at.ToString("HH:mm"),
                        };

                        if (i + 1 == lsstatus.Count)
                        {
                            if (timekt < DateTime.Now)
                                dMShowChart.end = timekt.ToString("HH:mm");
                            else
                                dMShowChart.end = DateTime.Now.ToString("HH:mm");
                        }
                        else
                            dMShowChart.end = lsstatus[i + 1].created_at.ToString("HH:mm");

                        dMShowChart.slstart = lscounter
                                                .Where(x => x.created_at <= lsstatus[i].created_at)
                                                .OrderByDescending(x => x.created_at)
                                                .FirstOrDefault()?.quantity ?? 0;

                        dMShowChart.slend = dMShowChart.slstart;

                        dschartError.Add(dMShowChart);
                        dschart.Add(dMShowChart);

                        //Tìm trạng thái MAY_CHAY gần nhất tiếp theo (nếu có)
                        for (int j = i + 1; j < lsstatus.Count; j++)
                        {
                            if (lsstatus[j].status == "MAY_DUNG" || lsstatus[j].status == "MAY_LOI")
                                break;
                            if (lsstatus[j].status == "MAY_CHAY" && j+1 != lsstatus.Count)
                            {
                                var dmMayChay = new DMShowChart()
                                {
                                    status = lsstatus[j].status,
                                    start = lsstatus[j].created_at.ToString("HH:mm"),
                                    slstart = lscounter
                                                .Where(x => x.created_at <= lsstatus[j].created_at)
                                                .OrderByDescending(x => x.created_at)
                                                .FirstOrDefault()?.quantity ?? 0
                                };

                                if (j + 1 == lsstatus.Count)
                                {
                                    if (timekt < DateTime.Now)
                                        dmMayChay.end = timekt.ToString("HH:mm");
                                    else
                                        dmMayChay.end = DateTime.Now.ToString("HH:mm");
                                    dmMayChay.slend = lscounter
                                                        .Where(x => x.created_at <= DateTime.Now && x.quantity > 0)
                                                        .OrderByDescending(x => x.created_at)
                                                        .FirstOrDefault()?.quantity ?? 0; ;
                                }
                                else
                                {
                                    dmMayChay.end = lsstatus[j + 1].created_at.ToString("HH:mm");
                                    dmMayChay.slend = lscounter
                                                        .Where(x => x.created_at <= lsstatus[j+1].created_at && x.quantity > 0)
                                                        .OrderByDescending(x => x.created_at)
                                                        .FirstOrDefault()?.quantity ?? 0;
                                }

                                dschart.Add(dmMayChay);

                                i = j - 1;
                                break;
                            }
                        }
                    }
                }
            }

            //show ACT+SuCo
            Series ACTSeries = chart1.Series["ACT"];
            ACTSeries.ChartType = SeriesChartType.Line;
            ACTSeries.Points.Clear();

            Series SuCoSeries = chart1.Series["SỰ CỐ"];
            SuCoSeries.ChartType = SeriesChartType.Line;
            SuCoSeries.Points.Clear();

            Series MAYDUNGSeries = chart1.Series["MÁY DỪNG"];
            MAYDUNGSeries.ChartType = SeriesChartType.Line;
            MAYDUNGSeries.Points.Clear();

            chart1.ChartAreas[0].AxisX.CustomLabels.Clear();
            List<DateTime> timePointsACT = new List<DateTime>();
            if (lsstatus.Count > 0)
            {
                foreach (var item in dschart)
                {
                    /*if (item.start == item.end) continue;*/ // Bỏ khoảng không hợp lệ

                    TimeSpan tsStart = TimeSpan.Parse(item.start);
                    TimeSpan tsEnd = TimeSpan.Parse(item.end);
                    var tscheck = TimeSpan.Parse(timekt.ToString("HH:mm:ss"));

                    // Nếu end <= start thì end là ngày hôm sau
                    DateTime startTime = DateTime.ParseExact($"{datenow:yyyy-MM-dd} {item.start}", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    DateTime endTime = DateTime.ParseExact($"{datenow:yyyy-MM-dd} {item.end}", "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);

                    if (TimeSpan.Parse(startTime.ToString("HH:mm:ss")) > tscheck)
                    {
                        startTime = startTime.AddDays(-1); // Nếu end < start thì thực sự là qua ngày hôm sau
                    }
                    if (TimeSpan.Parse(endTime.ToString("HH:mm:ss")) > tscheck)
                    {
                        endTime = endTime.AddDays(-1);
                    }

                    // Thêm 2 điểm cho line chart (line chỉ từ start đến end)
                    ACTSeries.Points.AddXY(startTime, item.slstart);
                    ACTSeries.Points.AddXY(endTime, item.slend);

                    
                    if (item.status == "MAY_LOI")
                    {
                        SuCoSeries.Points.AddXY(startTime, item.slstart);
                        SuCoSeries.Points.AddXY(endTime, item.slend);

                        var dungEnd = new DataPoint(endTime.ToOADate(), item.slend)
                        {
                            IsEmpty = true // Đảm bảo không vẽ thêm đoạn dư
                        };
                        SuCoSeries.Points.Add(dungEnd);
                    }

                    if (item.status == "MAY_DUNG" || item.status == "KHONG_CO_TRANG_THAI")
                    {
                        MAYDUNGSeries.Points.AddXY(startTime, item.slstart);
                        MAYDUNGSeries.Points.AddXY(endTime, item.slend);

                        var dungEnd = new DataPoint(endTime.ToOADate(), item.slend)
                        {
                            IsEmpty = true // Đảm bảo không vẽ thêm đoạn dư
                        };
                        MAYDUNGSeries.Points.Add(dungEnd);
                    }
                    // Gán label cho điểm cuối
                    var dpEnd = new DataPoint(endTime.ToOADate(), item.slend)
                    {
                        //Label = item.slend.ToString()
                    };
                    ACTSeries.Points.Add(dpEnd);

                    // Tạo custom label hiển thị trên trục X
                    var axisX2 = chart1.ChartAreas[0].AxisX;
                    var label = new CustomLabel
                    {
                        FromPosition = endTime.ToOADate() - 0.04,
                        ToPosition = endTime.ToOADate() + 0.04,
                        Text = $"{endTime:HH:mm}" 
                    };
                    axisX2.CustomLabels.Add(label);

                    timePointsACT.Add(endTime);
                        
                }

                // Gợi ý thêm để hiển thị rõ ràng hơn:
                var axX = chart1.ChartAreas[0].AxisX;
                axX.LabelStyle.Angle = -90;
                axX.MajorGrid.Enabled = false;
                axX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            }

            //config chart x 
            var axisX = chart1.ChartAreas[0].AxisX;
            axisX.LabelStyle.Format = "HH:mm";
            //axisX.IntervalType = DateTimeIntervalType.Hours;
            axisX.LabelStyle.Font = new Font("Segoe UI", 6, FontStyle.Regular);
            axisX.LabelStyle.Enabled = true;
            axisX.LabelStyle.Angle = -90;
            axisX.MajorGrid.Enabled = false;
            axisX.ScaleView.Zoomable = true;
            axisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            axisX.ScrollBar.Enabled = true;
            axisX.ScrollBar.IsPositionedInside = true;
            //axisX.Minimum = timebd.ToOADate();
            //axisX.Maximum = timekt.ToOADate();
            axisX.IsReversed = false;

            Axis axisX1 = chart1.ChartAreas[0].AxisX;
            foreach (var time in timePointsACT.OrderBy(x => x))
            {
                double pos = time.ToOADate();

                var label = new CustomLabel
                {
                    FromPosition = pos - 0.04,
                    ToPosition = pos + 0.04,
                    Text = time.ToString("HH:mm")
                };
                axisX1.CustomLabels.Add(label);

                var strip = new StripLine
                {
                    Interval = 0,
                    IntervalOffset = pos,
                    StripWidth = 0,
                    BorderColor = Color.FromArgb(70, Color.Black),
                    BorderDashStyle = ChartDashStyle.Dot,
                    BorderWidth = 1
                };
                axisX1.StripLines.Add(strip);
            }

            foreach (var time in timePoints.OrderBy(x => x))
            {
                //chart1.ChartAreas[0].AxisX.StripLines.Clear();
                double pos = time.ToOADate();

                // Gán CustomLabel rõ ràng
                var label = new CustomLabel
                {
                    FromPosition = pos - 0.04,
                    ToPosition = pos + 0.04,
                    Text = time.ToString("HH:mm")
                };
                axisX.CustomLabels.Add(label);

                // Kẻ strip line
                var strip = new StripLine
                {
                    Interval = 0,
                    IntervalOffset = pos,
                    StripWidth = 0,
                    BorderColor = Color.FromArgb(70, Color.Black),
                    BorderDashStyle = ChartDashStyle.Dot,
                    BorderWidth = 1
                };
                chart1.ChartAreas[0].AxisX.StripLines.Add(strip);
            }   

            //show sl tt plan + act
            double counttime = 0, sllbact = 0, sllbPlan = 0, timedungKH = 0;
            //counttime = gettime(getTimeCa, timebd, timekt);

            if (dschart.Count > 0)
            {
                lbACT.Text = Math.Round(dschart.LastOrDefault().slend,0).ToString();
                lbTotalACT.Text = Math.Round(dschart.LastOrDefault().slend, 0).ToString();
            }
            else
            {
                lbACT.Text = "0";
                lbTotalACT.Text = "0";
            }
            for (int i = 0; i< getTimeCa.Count; i++)
            {
                if (i+1 != getTimeCa.Count)
                {
                    if (DateTime.Now >= getTimeCa[i].todate && DateTime.Now < getTimeCa[i+1].fromdate)
                    {
                        timedungKH += (getTimeCa[i].todate - getTimeCa[i].fromdate).TotalSeconds;
                        break;
                    }
                    else if (DateTime.Now >= getTimeCa[i].fromdate && DateTime.Now <= getTimeCa[i].todate)
                    {
                        timedungKH += (DateTime.Now - getTimeCa[i].fromdate).TotalSeconds;
                        break;
                    }
                    else if (DateTime.Now >= getTimeCa[i].todate)
                    {
                        timedungKH += (getTimeCa[i].todate - getTimeCa[i].fromdate).TotalSeconds;
                    }
                }else if (i== getTimeCa.Count-1)
                {
                    timedungKH += (getTimeCa[i].todate - getTimeCa[i].fromdate).TotalSeconds;
                }
            }
            if (DateTime.Now >= timekt)
            {
                counttime = (timekt - timebd).TotalSeconds;
            }else
                counttime = (DateTime.Now - timebd).TotalSeconds;

            try
            {
                if (DateTime.Now > timebd)
                    lbPlan.Text = Math.Round((counttime - timedungKH) / timeCT, 0).ToString();
                else
                    lbPlan.Text = "0";
                double.TryParse(lbACT.Text, out sllbact);
                double.TryParse(lbPlan.Text, out sllbPlan);
                lbDIF.Text = (sllbact - sllbPlan).ToString();
                if (sllbact - sllbPlan >= 0)
                    lbDIF.ForeColor = Color.Blue;
                else
                    lbDIF.ForeColor = Color.Red;
                lbTotalPlan.Text = slkh.ToString();
                lbTotalDIF.Text = (int.Parse(lbTotalACT.Text) - int.Parse(lbTotalPlan.Text)).ToString();
                if ((int.Parse(lbTotalACT.Text) - int.Parse(lbTotalPlan.Text)) >= 0)
                    lbTotalDIF.ForeColor = Color.Blue;
                else
                    lbTotalDIF.ForeColor = Color.Red;
            }
            catch(Exception ex) { }
            
            // show table 1
            dgvDungThucTe.Rows.Clear();
            dgvDungThucTe.Font = new Font("Times New Roman", 9, FontStyle.Regular);
            dgvDungThucTe.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDungThucTe.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDungThucTe.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            int indexRow = 0;
            if (lsstatus.Count > 0)
            {
                for(int i = 0; i < lsstatus.Count; i++)
                {
                    if (lsstatus[i].status == "MAY_DUNG" || lsstatus[i].status == "MAY_LOI")
                    {
                        dgvDungThucTe.Rows.Add(1);
                        if (lsstatus[i].stoppagereason_id == -1)
                        {
                            dgvDungThucTe.Rows[indexRow].Cells[0].Value = lsstatus[i].shifttimetableexception_name;
                            dgvDungThucTe.Rows[indexRow].Cells[1].Value = lsstatus[i].created_at.ToString("HH:mm");
                            if (i+1 == lsstatus.Count)
                            {
                                
                                if (DateTime.Now <= timekt)
                                {
                                    dgvDungThucTe.Rows[indexRow].Cells[2].Value = DateTime.Now.ToString("HH:mm");
                                    dgvDungThucTe.Rows[indexRow].Cells[3].Value = Math.Round((DateTime.Now - lsstatus[i].created_at).TotalMinutes, 1).ToString() + "'";
                                }
                                else
                                {
                                    dgvDungThucTe.Rows[indexRow].Cells[2].Value = timekt.ToString("HH:mm");
                                    dgvDungThucTe.Rows[indexRow].Cells[3].Value = Math.Round((timekt - lsstatus[i].created_at).TotalMinutes, 1).ToString() + "'";
                                }
                            }
                            else
                            {
                                dgvDungThucTe.Rows[indexRow].Cells[2].Value = lsstatus[i + 1].created_at.ToString("HH:mm");
                                dgvDungThucTe.Rows[indexRow].Cells[3].Value = Math.Round((lsstatus[i + 1].created_at - lsstatus[i].created_at).TotalMinutes, 1).ToString() + "'";
                            }

                            //dgvDungThucTe.Rows[indexRow].Cells[4].Value = "Method";
                        }
                        else
                        {
                            dgvDungThucTe.Rows[indexRow].Cells[0].Value = lsstatus[i].stop_name;
                            dgvDungThucTe.Rows[indexRow].Cells[1].Value = lsstatus[i].created_at.ToString("HH:mm");
                            if (i + 1 == lsstatus.Count)
                            {
                                if (DateTime.Now <= timekt)
                                {
                                    dgvDungThucTe.Rows[indexRow].Cells[2].Value = DateTime.Now.ToString("HH:mm");
                                    dgvDungThucTe.Rows[indexRow].Cells[3].Value = Math.Round((DateTime.Now - lsstatus[i].created_at).TotalMinutes, 1).ToString() + "'";
                                }
                                else
                                {
                                    dgvDungThucTe.Rows[indexRow].Cells[2].Value = timekt.ToString("HH:mm");
                                    dgvDungThucTe.Rows[indexRow].Cells[3].Value = Math.Round((timekt - lsstatus[i].created_at).TotalMinutes,1).ToString() + "'";
                                }
                            }
                            else
                            {
                                dgvDungThucTe.Rows[indexRow].Cells[2].Value = lsstatus[i + 1].created_at.ToString("HH:mm");
                                dgvDungThucTe.Rows[indexRow].Cells[3].Value = Math.Round((lsstatus[i + 1].created_at - lsstatus[i].created_at).TotalMinutes, 1).ToString() + "'";
                            }
                            //dgvDungThucTe.Rows[i].Cells[4].Value = "Machine";
                        }
                        indexRow++;
                    }
                }
            }
            dgvDungThucTe.ClearSelection();
            dgvDungThucTe.CurrentCell = null;

            // show table 2
            dgvKeHoachAndThucTe.Rows.Clear();
            dgvKeHoachAndThucTe.Font = new Font("Times New Roman", 9, FontStyle.Regular);
            dgvKeHoachAndThucTe.Columns[1].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvKeHoachAndThucTe.Columns[2].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvKeHoachAndThucTe.Columns[3].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            indexRow = 0;
            double sumCol2 = 0, sumtgloi = 0;
            if (lsstatus.Count > 0 && getTimeCa.Count > 0)
            {
                for (int i = 0; i < getTimeCa.Count; i++)
                {
                    dgvKeHoachAndThucTe.Rows.Add(1);
                    dgvKeHoachAndThucTe.Rows[indexRow].Cells[0].Value = getTimeCa[i].namesc;
                    dgvKeHoachAndThucTe.Rows[indexRow].Cells[1].Value = (getTimeCa[i].todate - getTimeCa[i].fromdate).TotalMinutes.ToString();
                    var filteredList = lsstatus
                                        .Where(x => x.shifttimetableexception_id == getTimeCa[i].iddung)
                                        .ToList();
                    if (filteredList.Count > 0)
                    {
                        for (int j = 0; j < lsstatus.Count; j++)
                        {
                            if (lsstatus[j].status == "MAY_DUNG" && lsstatus[j].id == filteredList.FirstOrDefault().id)
                            {
                                if (j + 1 == lsstatus.Count)
                                {
                                    if (DateTime.Now <= timekt)
                                    {
                                        sumCol2 += Math.Round((DateTime.Now - lsstatus[j].created_at).TotalMinutes, 1);
                                        dgvKeHoachAndThucTe.Rows[indexRow].Cells[2].Value = Math.Round((DateTime.Now - lsstatus[j].created_at).TotalMinutes, 1).ToString();
                                        dgvKeHoachAndThucTe.Rows[indexRow].Cells[3].Value = Math.Round(((DateTime.Now - lsstatus[j].created_at).TotalMinutes - (getTimeCa[i].todate - getTimeCa[i].fromdate).TotalMinutes), 2).ToString();
                                    }
                                    else
                                    {
                                        sumCol2 += Math.Round((timekt - lsstatus[j].created_at).TotalMinutes, 1);
                                        dgvKeHoachAndThucTe.Rows[indexRow].Cells[2].Value = Math.Round((timekt - lsstatus[j].created_at).TotalMinutes, 1).ToString();
                                        dgvKeHoachAndThucTe.Rows[indexRow].Cells[3].Value = Math.Round(((timekt - lsstatus[j].created_at).TotalMinutes - (getTimeCa[i].todate - getTimeCa[i].fromdate).TotalMinutes), 2).ToString();
                                    }
                                    
                                }
                                else
                                {
                                    sumCol2 += Math.Round((lsstatus[j + 1].created_at - lsstatus[j].created_at).TotalMinutes, 1);
                                    dgvKeHoachAndThucTe.Rows[indexRow].Cells[2].Value = Math.Round((lsstatus[j + 1].created_at - lsstatus[j].created_at).TotalMinutes, 1).ToString();
                                    dgvKeHoachAndThucTe.Rows[indexRow].Cells[3].Value = Math.Round(((lsstatus[j + 1].created_at - lsstatus[j].created_at).TotalMinutes - (getTimeCa[i].todate - getTimeCa[i].fromdate).TotalMinutes), 1).ToString();
                                }
                                
                            }
                        }

                    }
                    else
                    {
                        dgvKeHoachAndThucTe.Rows[indexRow].Cells[2].Value = "0";
                        dgvKeHoachAndThucTe.Rows[indexRow].Cells[3].Value = (0 - (getTimeCa[i].todate - getTimeCa[i].fromdate).TotalMinutes).ToString();
                    }
                    indexRow++;
                }
                for (int i = 0; i < lsstatus.Count; i++)
                {
                    if (lsstatus[i].status == "MAY_LOI")
                    {
                        if (i + 1 == lsstatus.Count)
                        {
                            if (DateTime.Now <= timekt)
                            {
                                sumtgloi += Math.Round((DateTime.Now - lsstatus[i].created_at).TotalMinutes, 1);
                            }
                            else
                            {
                                sumtgloi += Math.Round((timekt - lsstatus[i].created_at).TotalMinutes, 1);
                            }
                            
                        }
                        else
                        {
                            sumtgloi += Math.Round((lsstatus[i + 1].created_at - lsstatus[i].created_at).TotalMinutes, 1);
                        }
                    }
                }
                dgvKeHoachAndThucTe.Rows.Add(1);
                dgvKeHoachAndThucTe.Rows[indexRow].Cells[0].Value = "Dừng đột xuất";
                dgvKeHoachAndThucTe.Rows[indexRow].Cells[1].Value = "0";
                dgvKeHoachAndThucTe.Rows[indexRow].Cells[2].Value = Math.Round(sumtgloi, 1).ToString();
                dgvKeHoachAndThucTe.Rows[indexRow].Cells[3].Value = Math.Round(sumtgloi, 1).ToString();
            }

            //show label sumtime
            try
            {
                lbTimePlanStart.Text = timebd.ToString("HH:mm");
                if (lscounter.Count > 0)
                {
                    lbTimeACTStart.Text = lscounter.FirstOrDefault().created_at.ToString("HH:mm");
                    lbTimeACTEnd.Text = lscounter.LastOrDefault().created_at.ToString("HH:mm");
                }
                else
                {
                    //MessageBox.Show("Cannot get counter", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    lbTimeACTStart.Text = timebd.ToString("HH:mm");
                    lbTimeACTEnd.Text = timekt.ToString("HH:mm");
                }

                lbTimePlanEnd.Text = timekt.ToString("HH:mm");
                lbTimePlanTotal.Text = (timekt - timebd).TotalMinutes.ToString() + "'";
                lbTimeACTTotal.Text = Math.Round((sumtimeact / 60), 0).ToString() + "'";

                if (DateTime.Now >= timekt)
                {
                    double sumtimekh = 0;
                    if (timekt.Date != timebd.Date)
                    {
                        //timebd = timebd.AddDays(-1);
                        if ((timekt - timebd).TotalHours < 10)
                            sumtimekh = 435;
                        else
                            sumtimekh = 615;
                    }
                    else
                    {
                        if ((timekt - timebd).TotalHours < 10)
                            sumtimekh = 480;
                        else
                            sumtimekh = 660;
                    }
                    double a = (timekt - timebd).TotalMinutes - 60;
                    double b = double.Parse(lbTotalACT.Text);
                    lbTiLeChayChuyen.Text = Math.Round((b*35 / (sumtimekh*60)) * 100, 1).ToString() + "%";
                    //lbTiLeChayChuyen.Text = Math.Round(((a - (sumCol2+ sumtgloi)) / (timekt - timebd).TotalSeconds) * 100, 1).ToString() + "%";
                    if (Math.Round((sumtimeact / 60), 0) > (timekt - timebd).TotalMinutes)
                        lbTyLeHT.Text = "Không đạt";
                    else
                        lbTyLeHT.Text = "Đạt";
                }
                else
                {
                    lbTiLeChayChuyen.Text = "";
                    lbTyLeHT.Text = "";
                }
            }
            catch (Exception ex) { }

        }

        public static DMStatusinfo CloneStatus(DMStatusinfo source)
        {
            return  new DMStatusinfo
            {
                id = source.id,
                machinecode = source.machinecode,
                status = source.status,
                created_at = source.created_at,
                error_code = source.error_code,
                stoppagereason_id = source.stoppagereason_id,
                shifttimetableexception_id = source.shifttimetableexception_id,
                stop_name = source.stop_name,
                shifttimetableexception_name = source.shifttimetableexception_name,
                fromdate = source.fromdate,
                todate = source.todate
            };
        }

        private double gettime(List<DMTimeByCa> getTimeCa, DateTime timebd, DateTime timekt)
        {
            double counttime = 0, sllbact = 0, sllbPlan = 0;
            DateTime timenow = DateTime.Now;
            timenow = new DateTime(
                        DateTime.Now.Year,
                        DateTime.Now.Month,
                        DateTime.Now.Day,
                        DateTime.Now.Hour,
                        DateTime.Now.Minute,
                        0
                    );
            if (getTimeCa.Count > 0)
            {
                for (var i = 0; i < getTimeCa.Count; i++)
                {
                    if (getTimeCa[i].fromdate < timenow && getTimeCa[i].todate < timenow)
                    {
                        counttime = (getTimeCa[i].todate - getTimeCa[i].fromdate).TotalSeconds + counttime;
                    }
                    else if (getTimeCa[i].fromdate > timenow)
                    {
                        //counttime = (DateTime.Now - getTimeCa[i-1].todate).TotalSeconds + counttime;
                        break;
                    }
                }
                
                if (timenow > timekt)
                    counttime = (timekt - timebd).TotalSeconds - counttime;
                else
                    counttime = (timenow - timebd).TotalSeconds - counttime;
            }
            return counttime;
        }

        private void cbbChuyen_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbChuyen.DataSource != null)
            {
                List<DMModel> lsmodel = frmDashboardTTSXServices.getDSModel(datenow, cbbChuyen.SelectedValue.ToString());
                if (lsmodel.Count > 0)
                {
                    cbbModel.ValueMember = "id";
                    cbbModel.DisplayMember = "name";
                    cbbModel.DataSource = lsmodel;
                    cbbModel.SelectedIndex = 0;

                    List<DMShift> lsca = frmDashboardTTSXServices.getIdCa(datenow, cbbChuyen.SelectedValue.ToString());
                    if (lsca.Count > 0)
                    {
                        cbbCa.ValueMember = "id";
                        cbbCa.DisplayMember = "name";
                        cbbCa.DataSource = lsca;
                        cbbCa.SelectedIndex = 0;
                    }

                    loaddata(null, null);
                }
            }
        }

        private List<DMStatusinfo> statusFilter(List<DMStatusinfo> lsstatus1, List<DMStatusinfo> lsstatus, DateTime timekt)
        {
            
            return lsstatus;
        }

        private void frmDashboardTTSX_Load(object sender, EventArgs e)
        {
            InitializeComponent();
            //dtpSearch.Value = DateTime.Now;
            
            //loaddata(null, null);
        }

        private void dtpSearch_CloseUp(object sender, EventArgs e)
        {
            
        }

        private void cbbCa_SelectedIndexChanged(object sender, EventArgs e)
        {
            loaddata(null, null);
        }

        private void frmDashboardTTSX_FormClosing(object sender, FormClosingEventArgs e)
        {
            isFormClosing = true;

            if (tRefreshData != null)
            {
                tRefreshData.Stop();
                tRefreshData.Dispose();
                tRefreshData = null;
            }
        }

        private void dgvDungThucTe_SelectionChanged(object sender, EventArgs e)
        {
            dgvDungThucTe.ClearSelection();
            dgvDungThucTe.CurrentCell = null;
        }

        private void dgvKeHoachAndThucTe_SelectionChanged(object sender, EventArgs e)
        {
            dgvKeHoachAndThucTe.ClearSelection();
            dgvKeHoachAndThucTe.CurrentCell = null;
        }

        private void btnBackDay_Click(object sender, EventArgs e)
        {
            datenow = datenow.AddDays(-1);
            dtpSearch.Value = datenow;
        }

        private void btnNextDay_Click(object sender, EventArgs e)
        {
            datenow = datenow.AddDays(1);
            dtpSearch.Value = datenow;
        }

        private void dtpSearch_ValueChanged(object sender, EventArgs e)
        {
            datenow = dtpSearch.Value;
            List<DMChuyen> lsChuyen = frmDashboardTTSXServices.getDSChuyen(datenow);
            if (lsChuyen.Count > 0)
            {
                cbbChuyen.ValueMember = "id";
                cbbChuyen.DisplayMember = "name";
                cbbChuyen.DataSource = lsChuyen;
                cbbChuyen.SelectedIndex = 0;
            }
            else
            {
                cbbChuyen.DataSource = null;
            }
            if (cbbChuyen.DataSource != null)
            {
                List<DMModel> lsmodel = frmDashboardTTSXServices.getDSModel(datenow, cbbChuyen.SelectedValue.ToString());
                if (lsmodel.Count > 0)
                {
                    cbbModel.ValueMember = "id";
                    cbbModel.DisplayMember = "name";
                    cbbModel.DataSource = lsmodel;
                    cbbModel.SelectedIndex = 0;

                    List<DMShift> lsca = frmDashboardTTSXServices.getIdCa(datenow, cbbChuyen.SelectedValue.ToString());
                    if (lsca.Count > 0)
                    {
                        cbbCa.ValueMember = "id";
                        cbbCa.DisplayMember = "name";
                        cbbCa.DataSource = lsca;
                        cbbCa.SelectedIndex = 0;
                    }

                    loaddata(null, null);
                }
            }
        }
    }
}