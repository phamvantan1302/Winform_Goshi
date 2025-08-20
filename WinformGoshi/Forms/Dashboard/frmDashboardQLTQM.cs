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
using static WinformGoshi.Models.dashboard.DashboardTTSX;
using WinformGoshi.Sevices.dashboard;
using System.Reflection;
using System.Collections;
using WinformGoshi.Models.dashboard;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace WinformGoshi.Forms.Dashboard
{
    public partial class frmDashboardQLTQM : Form
    {
        private System.Windows.Forms.Timer tRefreshData;
        public frmDashboardQLTQM()
        {
            InitializeComponent();
            loaddata(null, null);
            LBThang.Text = "Tháng " + DateTime.Now.Month.ToString();
            //tRefreshData = new System.Windows.Forms.Timer();
            //tRefreshData.Tick += new EventHandler(loaddata);
            //tRefreshData.Interval = 30000;
            //tRefreshData.Enabled = true;
            
        }

        private void loaddata(object sender, EventArgs e)
        {
            Series planSeries = chart1.Series["plan"];
            planSeries.Points.Clear();
            Series actSeries = chart1.Series["act"];
            actSeries.Points.Clear();
            Series tyleSeries = chart1.Series["tyle"];
            tyleSeries.Points.Clear();
            chart1.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            chart1.ChartAreas[0].AxisY2.MajorGrid.Enabled = false;
            planSeries.YAxisType = AxisType.Primary;
            actSeries.YAxisType = AxisType.Primary;
            tyleSeries.YAxisType = AxisType.Secondary;

            //Data
            string valueThu = "", bdau = "", kthuc = "";
            double sumtimeact = 0, slTTtrongngay = 0;
            DateTime timebd = DateTime.Now, timekt = DateTime.Now, date = DateTime.Now;
            List<DMTimeByCa> getTimeCa = frmDashboardQLTQMServices.getDataCaKH(date);
            List<DMStatusinfo> lsstatus = frmDashboardQLTQMServices.getDSStatusinfo(date);
            List<DMCounterinfo> lscounter = frmDashboardQLTQMServices.getDSCounterinfo(date);
            List<DMOrder> lsorder = frmDashboardQLTQMServices.getDataOrder(DateTime.Now);
            List<DMPlan> lsplan = new List<DMPlan>();
            DMPlan dMPlan = new DMPlan();
            List<DMPlan> lsact = new List<DMPlan>();
            DMPlan dMact = new DMPlan();
            List<dataViewTable> lsLV = new List<dataViewTable>();
            dataViewTable dMLV = new dataViewTable();

            chart1.ChartAreas[0].AxisY2.Minimum = 0;
            chart1.ChartAreas[0].AxisY2.Maximum = 150;
            Title titleY2 = new Title();
            titleY2.Text = "(%)";
            titleY2.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            titleY2.ForeColor = Color.Black;
            titleY2.IsDockedInsideChartArea = false; // bỏ docking
            titleY2.Position.Auto = false;
            titleY2.Position.X = 97;  // phần trăm chiều rộng chart (càng lớn càng sát phải)
            titleY2.Position.Y = 9;   // phần trăm chiều cao chart (càng nhỏ càng sát trên)
            chart1.Titles.Add(titleY2);

            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Maximum = 30;
            Title titleY = new Title();
            titleY.Text = "(h)";
            titleY.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            titleY.ForeColor = Color.Black;
            titleY.IsDockedInsideChartArea = false; // bỏ docking
            titleY.Position.Auto = false;
            titleY.Position.X = 3;  // phần trăm chiều rộng chart (càng lớn càng sát phải)
            titleY.Position.Y = 9;   // phần trăm chiều cao chart (càng nhỏ càng sát trên)
            chart1.Titles.Add(titleY);

            chart1.ChartAreas[0].AxisX.Minimum = 0.5;
            chart1.ChartAreas[0].AxisX.Maximum = 31.5;
            chart1.ChartAreas[0].AxisX.Interval = 1;
            chart1.ChartAreas[0].AxisX.LabelStyle.IntervalOffset = 0.5;
            chart1.ChartAreas[0].AxisX.LabelStyle.Enabled = false;

            //planSeries.IsXValueIndexed = true;
            //actSeries.IsXValueIndexed = true;
            //tyleSeries.IsXValueIndexed = true;
            //chart1.ChartAreas[0].AxisX.IsMarginVisible = true;

            if (getTimeCa.Count > 0)
            {
                //Data
                for (int day = 1; day <= 31; day++)
                {
                    DateTime currentDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day);
                    double sumtimeca = 0;
                    sumtimeact = 0;
                    slTTtrongngay = 0;

                    var dailyData = lsorder
                    .Where(x => x.fromDate.Date == currentDate)
                    .Select(x => (x.plannedquantity, x.shift_id))
                    .ToList();
                    var totalInDay = lsorder
                        .Where(x => x.fromDate.Date == currentDate)
                        .Sum(x => x.plannedquantity);

                    if (dailyData.Count > 0)
                    {
                        foreach (var dd in dailyData)
                        {
                            var timebdkt = getTimeCa.Where(x => x.id == dd.shift_id).ToList();
                            if (timebdkt.Count == 0)
                                continue;
                            bdau = timebdkt.FirstOrDefault().mondayhours.Split('-')[0];
                            kthuc = timebdkt.FirstOrDefault().mondayhours.Split('-')[1];
                            DateTime bdTime, ktTime;
                            if (DateTime.TryParse(bdau, out bdTime) && DateTime.TryParse(kthuc, out ktTime))
                            {
                                timebd = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, bdTime.Hour, bdTime.Minute, 0);
                                timekt = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, ktTime.Hour, ktTime.Minute, 0);
                                if (ktTime < bdTime)
                                {
                                    timebd = timebd.AddDays(-1);
                                }
                                sumtimeca += (timekt - timebd).TotalSeconds;

                                //act
                                var listTrongNgay = lsstatus
                                    .Where(x => x.created_at >= timebd && x.created_at<= timekt)
                                    .ToList();
                                for (int i = 0; i < listTrongNgay.Count; i++)
                                {
                                    if (listTrongNgay[i].status == "MAY_CHAY")
                                    {
                                        if (i + 1 == listTrongNgay.Count)
                                        {
                                            if (DateTime.Now < timekt)
                                            {
                                                sumtimeact = (DateTime.Now - listTrongNgay[i].created_at).TotalSeconds + sumtimeact;
                                            }
                                        }
                                        else
                                            sumtimeact = (listTrongNgay[i + 1].created_at - listTrongNgay[i].created_at).TotalSeconds + sumtimeact;
                                    }
                                }

                                var listSLTrongNgay = frmDashboardQLTQMServices.getSLByNgay(timebd, timekt);
                                if(listSLTrongNgay.Count > 0)
                                    slTTtrongngay += listSLTrongNgay.LastOrDefault().quantity;
                            }

                        }
                    }

                    //act
                    dMact = new DMPlan()
                    {
                        ngay = day,
                        gio = Math.Round(sumtimeact / 3600, 1)
                    };
                    lsact.Add(dMact);

                    dMPlan = new DMPlan
                    {
                        ngay = day,
                        gio = Math.Round(sumtimeca / 3600, 1)
                    };
                    lsplan.Add(dMPlan);

                    dMLV = new dataViewTable()
                    {
                        ngay = day,
                        slplan = totalInDay,
                        tgkehoach = Math.Round(sumtimeca / 3600, 1),
                        slact = slTTtrongngay,
                        tgianTT = Math.Round(sumtimeact / 3600, 1)
                    };
                    dMLV.sldiff = dMLV.slact - dMLV.slplan;
                    dMLV.chechlech = Math.Round(dMLV.tgianTT - dMLV.tgkehoach, 1);
                    if (dMLV.tgkehoach != 0)
                        dMLV.tyleht = Math.Round((dMLV.tgianTT / dMLV.tgkehoach) * 100, 0);
                    else
                        dMLV.tyleht = 0;
                    lsLV.Add(dMLV);
                }

                // show plan
                if (lsplan.Count > 0)
                {
                    foreach (var it in lsplan)
                    {
                        planSeries.Points.AddXY(it.ngay, it.gio);
                        planSeries.ToolTip = (it.gio).ToString();
                    }
                }
                //show act
                if (lsact.Count > 0)
                {
                    foreach (var it in lsact)
                    {
                        actSeries.Points.AddXY(it.ngay, it.gio);
                        actSeries.ToolTip = (it.gio).ToString();
                    }
                }

                //show tyle
                tyleSeries.EmptyPointStyle.BorderDashStyle = ChartDashStyle.Dash;
                tyleSeries.EmptyPointStyle.MarkerStyle = MarkerStyle.Cross;
                tyleSeries.IsValueShownAsLabel = true;

                for (int day = 1; day <= 31; day++)
                {
                    var plan = lsplan.FirstOrDefault(x => x.ngay == day);
                    var act = lsact.FirstOrDefault(x => x.ngay == day);

                    if (plan != null && act != null && act.gio != 0)
                    {
                        double tyle = (double)act.gio / plan.gio * 100;
                        DataPoint point = new DataPoint();
                        point.SetValueXY(day, tyle);
                        point.Label = $"{Math.Round(tyle, 0)}%";
                        point.LabelForeColor = Color.Green;
                        tyleSeries.Points.Add(point);
                    }
                    else
                    {
                        DataPoint emptyPoint = new DataPoint(day, 0);
                        //emptyPoint.SetValueXY(day, 0);
                        emptyPoint.IsEmpty = true;
                        tyleSeries.Points.Add(emptyPoint);
                    }
                }
            }

            // Show LV
            showLV(lsstatus, getTimeCa, lsLV);

        }

        private void showLV(List<DMStatusinfo> lsstatus, List<DMTimeByCa> getTimeCa, List<dataViewTable> lsLV)
        {
            // check data
            if (lsLV.Count <= 0)
                return;
            //show

            dgv.Font = new Font("Segoe UI", 8, FontStyle.Regular);
            dgv.Rows.Clear();
            dgv.Rows.Add(8);

            dgv.Rows[0].Cells[0].Value = "Plan";
            for (int i = 0; i< lsLV.Count; i++)
            {
                dgv.Rows[0].Cells[i+1].Value = lsLV[i].slplan.ToString();
            }
            dgv.Rows[0].Cells[32].Value = lsLV.Sum(x => x.slplan).ToString();

            dgv.Rows[1].Cells[0].Value = "Act";
            for (int i = 0; i < lsLV.Count; i++)
            {
                dgv.Rows[1].Cells[i + 1].Value = lsLV[i].slact.ToString();
            }
            dgv.Rows[1].Cells[32].Value = lsLV.Sum(x => x.slact).ToString();

            dgv.Rows[2].Cells[0].Value = "Diff";
            for (int i = 0; i < lsLV.Count; i++)
            {
                dgv.Rows[2].Cells[i + 1].Value = lsLV[i].sldiff.ToString();
            }
            dgv.Rows[2].Cells[32].Value = lsLV.Sum(x => x.sldiff).ToString();

            dgv.Rows[3].Cells[0].Value = "TG SX theo KH (h)";
            for (int i = 0; i < lsLV.Count; i++)
            {
                dgv.Rows[3].Cells[i + 1].Value = lsLV[i].tgkehoach.ToString();
            }
            dgv.Rows[3].Cells[32].Value = lsLV.Sum(x => x.tgkehoach).ToString();

            dgv.Rows[4].Cells[0].Value = "TG dừng chuyền";
            for (int i = 0; i < lsLV.Count; i++)
            {
                dgv.Rows[4].Cells[i + 1].Value = lsLV[i].tgiandung.ToString();
            }
            dgv.Rows[4].Cells[32].Value = lsLV.Sum(x => x.tgiandung).ToString();

            dgv.Rows[5].Cells[0].Value = "TG SX thực tế (h)";
            for (int i = 0; i < lsLV.Count; i++)
            {
                dgv.Rows[5].Cells[i + 1].Value = lsLV[i].tgianTT.ToString();
            }
            dgv.Rows[5].Cells[32].Value = lsLV.Sum(x => x.tgianTT).ToString();

            dgv.Rows[6].Cells[0].Value = "Chênh lệch (h)";
            for (int i = 0; i < lsLV.Count; i++)
            {
                dgv.Rows[6].Cells[i + 1].Value = lsLV[i].chechlech.ToString();
            }
            dgv.Rows[6].Cells[32].Value = lsLV.Sum(x => x.chechlech).ToString();

            dgv.Rows[7].Cells[0].Value = "Tỷ lệ hoàn thành";
            for (int i = 0; i < lsLV.Count; i++)
            {
                dgv.Rows[7].Cells[i + 1].Value = lsLV[i].tyleht.ToString() + "%";
            }

        }

        private void dgv_SelectionChanged(object sender, EventArgs e)
        {
            dgv.ClearSelection();
            dgv.CurrentCell = null;
        }
    }
}
