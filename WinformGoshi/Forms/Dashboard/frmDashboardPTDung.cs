using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using WinformGoshi.Models.dashboard;
using WinformGoshi.Sevices.dashboard;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static WinformGoshi.Models.dashboard.DashboardTTSX;

namespace WinformGoshi.Forms.Dashboard
{
    public partial class frmDashboardPTDung : Form
    {
        private System.Windows.Forms.Timer tRefreshData;
        private bool isFormClosing = false;
        public frmDashboardPTDung()
        {
            InitializeComponent();
            tRefreshData = new System.Windows.Forms.Timer();
            tRefreshData.Tick += new EventHandler(loaddashboard1);
            tRefreshData.Interval = 60000;
            tRefreshData.Enabled = true;
            loaddashboard1(null, null);
            loaddashboard2(null, null);
        }

        private void loaddashboard1(object sender, EventArgs e)
        {
            if (isFormClosing || chart1 == null || this.IsDisposed)
                return;
            //dtpDate.Value = new DateTime(2025, 09, 08);
            double sumdung = 0;
            string mmtb = "";
            DateTime toDate = dtpDate.Value, fromDate = dtpDate.Value;
            Series pieSeries1 = chart1.Series["Series1"];
            pieSeries1.Points.Clear();
            List<DMOrder> lsorder = frmDashboardPTDungServices.getDataOrder(dtpDate.Value, true);
            List<DMStatusinfo> lschart1 = new List<DMStatusinfo>();
            List<DMViewPie> lsShowChart = new List<DMViewPie>();
            DMViewPie dsit = null;
            // dataview chart 1
            //lschart1 = filterStatusChart1(lsorder);

            foreach (var it in lsorder)
            {
                toDate = it.toDate;
                fromDate = it.fromDate;

                if (toDate < fromDate)
                {
                    if ((toDate - fromDate).TotalHours < 10)
                        sumdung = 435;
                    else
                        sumdung = 615;
                }
                else
                {
                    if ((toDate - fromDate).TotalHours < 10)
                        sumdung = 480;
                    else
                        sumdung = 660;
                }

                List<DMStatusinfo> lsstatus1 = frmDashboardPTDungServices.getDSStatusinfo1(fromDate, toDate);
                if (lsstatus1.Count > 0)
                {
                    DMViewPie rcchart1 = new DMViewPie();

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

                            if (duration == 0 && DateTime.Now < toDate)
                            {
                                duration = (DateTime.Now - current.created_at).TotalSeconds;
                            }

                            if (duration > 120)
                            {
                                // Thêm trạng thái MAY_DUNG
                                lschart1.Add(CloneStatus(current));
                                //lschart1.Add(CloneStatus(nextMC));
                                sumdung += duration;
                                i++;
                            }
                        }

                    }
                }
            }

            if (lschart1.Count == 0)
                return;

            for (int i = 0; i < lschart1.Count; i++)
            {
                double sldung = 0;
                if (i != lschart1.Count-1)
                {
                    sldung = (lschart1[i + 1].created_at - lschart1[i].created_at).TotalSeconds;
                }
                dsit = new DMViewPie()
                {
                    name = lschart1[i].shifttimetableexception_name,
                    sumsl = sldung
                };
                lsShowChart.Add(dsit);
            }

            var groupedList1 = lsShowChart
                    .GroupBy(x => x.name)
                    .Select(g => new DMViewPie
                    {
                        name = g.Key,
                        sumsl = g.Sum(x => x.sumsl)
                    })
                    .ToList();
            double tongtgiandung = groupedList1.Sum(x => x.sumsl);

            // show chart 1
            foreach (var item in groupedList1)
            {
                if (double.IsNaN(item.sumsl) || double.IsInfinity(item.sumsl))
                    continue;
                if (!string.IsNullOrEmpty(item.name))
                {
                    int pointIndex = pieSeries1.Points.AddXY(item.name, Math.Round((double.Parse(item.sumsl.ToString()) / tongtgiandung) * 100, 0));
                    pieSeries1.Points[pointIndex].Label = $"{Math.Round((double.Parse(item.sumsl.ToString()) / tongtgiandung) * 100, 0)}%";
                    pieSeries1.Points[pointIndex].LegendText = item.name;
                }
                else
                {
                    int pointIndex = pieSeries1.Points.AddXY("Khác", Math.Round((double.Parse(item.sumsl.ToString()) / tongtgiandung) * 100, 0));
                    pieSeries1.Points[pointIndex].Label = $"{Math.Round((double.Parse(item.sumsl.ToString()) / tongtgiandung) * 100, 0)}%";
                    pieSeries1.Points[pointIndex].LegendText = "Khác";
                }
            }
            pieSeries1.IsValueShownAsLabel = true;
            pieSeries1.Font = new Font("Times New Roman", 12, FontStyle.Regular);

        }

        private void loaddashboard2(object sender, EventArgs e)
        {
            double tongerror = 0, sumdung = 0;
            DateTime toDate = DateTime.Now, fromDate = DateTime.Now;
            Series pieSeriesDung = chart2.Series["Series1"];
            pieSeriesDung.Points.Clear();

            //List<DMStatusinfo> lsstatus2 = frmDashboardPTDungServices.getDSStatusinfo2(dtpMonth.Value);
            List<DMStatusinfo> lschart2 = new List<DMStatusinfo>();
            List<DMViewPie> lsShowChart = new List<DMViewPie>();
            DMViewPie dsit = null;
            List<DMOrder> lsorder1 = frmDashboardPTDungServices.getDataOrder(toDate, false);

            int daysInMonth = DateTime.DaysInMonth(dtpMonth.Value.Year, dtpMonth.Value.Month);
            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime dateSearch = new DateTime(dtpMonth.Value.Year, dtpMonth.Value.Month, day);

                var lsorder = lsorder1.Where(x=> x.toDate.ToString("yyyy-MM-dd") == dateSearch.ToString("yyyy-MM-dd")).ToList();

                foreach (var it in lsorder)
                {
                    toDate = it.toDate;
                    fromDate = it.fromDate;

                    if (toDate < fromDate)
                    {
                        if ((toDate - fromDate).TotalHours < 10)
                            sumdung = 435;
                        else
                            sumdung = 615;
                    }
                    else
                    {
                        if ((toDate - fromDate).TotalHours < 10)
                            sumdung = 480;
                        else
                            sumdung = 660;
                    }

                    List<DMStatusinfo> lsstatus1 = frmDashboardPTDungServices.getDSStatusinfo1(fromDate, toDate);
                    if (lsstatus1.Count > 0)
                    {
                        DMViewPie rcchart1 = new DMViewPie();

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

                                if (duration == 0 && DateTime.Now < toDate)
                                {
                                    duration = (DateTime.Now - current.created_at).TotalSeconds;
                                }

                                if (duration > 120)
                                {
                                    // Thêm trạng thái MAY_DUNG
                                    lschart2.Add(CloneStatus(current));
                                    sumdung += duration;
                                    i++;
                                }
                            }

                        }
                    }
                }
            }

            if (lschart2.Count == 0)
                return;

            for (int i = 0; i < lschart2.Count; i++)
            {
                double sldung = 0;
                if (i != lschart2.Count-1)
                {
                    sldung = (lschart2[i + 1].created_at - lschart2[i].created_at).TotalSeconds;
                }
                dsit = new DMViewPie()
                {
                    name = lschart2[i].shifttimetableexception_name,
                    sumsl = sldung
                };
                lsShowChart.Add(dsit);
            }

            var groupedList = lsShowChart
                    .GroupBy(x => x.name)
                    .Select(g => new DMViewPie
                    {
                        name = g.Key,
                        sumsl = g.Sum(x => x.sumsl)
                    })
                    .ToList();
            double tongtgiandung = groupedList.Sum(x=> x.sumsl);
            //chart 2
            foreach (var item in groupedList)
            {
                if (double.IsNaN(item.sumsl) || double.IsInfinity(item.sumsl))
                    continue;
                if (!string.IsNullOrEmpty(item.name))
                {
                    int pointIndex = pieSeriesDung.Points.AddXY(item.name, Math.Round((double.Parse(item.sumsl.ToString()) / tongtgiandung) * 100, 0));
                    pieSeriesDung.Points[pointIndex].Label = $"{Math.Round((double.Parse(item.sumsl.ToString()) / tongtgiandung) * 100, 0)}%";
                    pieSeriesDung.Points[pointIndex].LegendText = item.name;
                }
                else
                {
                    int pointIndex = pieSeriesDung.Points.AddXY("Khác", Math.Round((double.Parse(item.sumsl.ToString()) / tongtgiandung) * 100, 0));
                    pieSeriesDung.Points[pointIndex].Label = $"{Math.Round((double.Parse(item.sumsl.ToString()) / tongtgiandung) * 100, 0)}%";
                    pieSeriesDung.Points[pointIndex].LegendText = "Khác";
                }
            }
            pieSeriesDung.IsValueShownAsLabel = true;
            pieSeriesDung.Font = new Font("Times New Roman", 12, FontStyle.Regular);
        }

        public static DMStatusinfo CloneStatus(DMStatusinfo source)
        {
            return new DMStatusinfo
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

        private void frmDashboardPTDung_FormClosing(object sender, FormClosingEventArgs e)
        {
            isFormClosing = true;

            if (tRefreshData != null)
            {
                tRefreshData.Stop();
                tRefreshData.Dispose();
                tRefreshData = null;
            }
        }

        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            loaddashboard1(null, null);
        }

        private void dtpMonth_ValueChanged(object sender, EventArgs e)
        {
            loaddashboard2(null, null);
        }
    }
}
