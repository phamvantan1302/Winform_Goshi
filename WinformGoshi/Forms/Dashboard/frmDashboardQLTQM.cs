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
            dtpSearch.Value = DateTime.Now;
            //tRefreshData = new System.Windows.Forms.Timer();
            //tRefreshData.Tick += new EventHandler(loaddata);
            //tRefreshData.Interval = 30000;
            //tRefreshData.Enabled = true;
            
        }

        private void loaddata(object sender, EventArgs e)
        {
            Series actSeries = chart1.Series["ttdem"];
            Series planSeries = chart1.Series["ttngay"];
            planSeries.Points.Clear();
            actSeries.Points.Clear();
            Series tyleSeries = chart1.Series["tyle"];
            tyleSeries.Points.Clear();
            Series tylekhSeries = chart1.Series["tylekh"];
            tylekhSeries.Points.Clear();
            chart1.ChartAreas[0].AxisY2.Enabled = AxisEnabled.True;
            chart1.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            chart1.ChartAreas[0].AxisY2.MajorGrid.Enabled = false;
            planSeries.YAxisType = AxisType.Primary;
            actSeries.YAxisType = AxisType.Primary;
            tyleSeries.YAxisType = AxisType.Secondary;
            tylekhSeries.YAxisType = AxisType.Secondary;

            actSeries.BorderColor = Color.Silver;
            actSeries.BorderWidth = 1;

            planSeries.BorderColor = Color.Silver;
            planSeries.BorderWidth = 1;

            //Data
            string valueThu = "", bdau = "", kthuc = "";
            double sumtimeact = 0, slTTtrongngay = 0;
            DateTime timebd = dtpSearch.Value, timekt = dtpSearch.Value, date = dtpSearch.Value;
            List<DMTimeByCa> getTimeCa = frmDashboardQLTQMServices.getDataCaKH(date);
            List<DMStatusinfo> lsstatus = frmDashboardQLTQMServices.getDSStatusinfo(date);
            List<DMCounterinfo> lscounter = frmDashboardQLTQMServices.getDSCounterinfo(date);
            List<DMOrder> lsorder = frmDashboardQLTQMServices.getDataOrder(dtpSearch.Value);
            List<DMPlan> lsacthc = new List<DMPlan>();
            DMPlan dMacthc = new DMPlan();
            List<DMPlan> lsactd = new List<DMPlan>();
            DMPlan dMactd = new DMPlan();
            List<dataViewTable> lsLV = new List<dataViewTable>();
            dataViewTable dMLV = new dataViewTable();

            chart1.ChartAreas[0].AxisY2.Minimum = 0;
            chart1.ChartAreas[0].AxisY2.Maximum = 100;
            chart1.ChartAreas[0].AxisY2.Interval = 10;
            Title titleY2 = new Title();
            titleY2.Text = "(%)";
            titleY2.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            titleY2.ForeColor = Color.Black;
            titleY2.IsDockedInsideChartArea = false; // bỏ docking
            titleY2.Position.Auto = false;
            titleY2.Position.X = 96;  // phần trăm chiều rộng chart (càng lớn càng sát phải)
            titleY2.Position.Y = 2;   // phần trăm chiều cao chart (càng nhỏ càng sát trên)
            chart1.Titles.Add(titleY2);

            chart1.ChartAreas[0].AxisY.Minimum = 0;
            chart1.ChartAreas[0].AxisY.Maximum = 2000;
            chart1.ChartAreas[0].AxisY.Interval = 200;

            //fix loi hien thi
            chart1.ChartAreas[0].AxisY.IsStartedFromZero = true;
            chart1.ChartAreas[0].AxisY.ScaleBreakStyle.Enabled = false;
            chart1.ChartAreas[0].RecalculateAxesScale();

            Title titleY = new Title();
            titleY.Text = "(pcs)";
            titleY.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            titleY.ForeColor = Color.Black;
            titleY.IsDockedInsideChartArea = false; // bỏ docking
            titleY.Position.Auto = false;
            titleY.Position.X = 5;  // phần trăm chiều rộng chart (càng lớn càng sát phải)
            titleY.Position.Y = 2;   // phần trăm chiều cao chart (càng nhỏ càng sát trên)
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
                    double sumtimeca = 0, sumtimekh = 0, totalslca = 0;
                    sumtimeact = 0;
                    slTTtrongngay = 0;
                    string ca = "";
                    bool checkca = false;

                    // TH date > date trong tháng
                    if (day > DateTime.DaysInMonth(date.Year, date.Month))
                    {
                        dMLV = new dataViewTable()
                        {
                            ngay = day,
                            slplan = totalslca,
                            slact = slTTtrongngay,
                            calv = "S4",
                            tyleht = "",
                            tylecc = 0
                        };
                        lsLV.Add(dMLV);
                        dMLV = new dataViewTable()
                        {
                            ngay = day,
                            slplan = totalslca,
                            slact = slTTtrongngay,
                            calv = "HC",
                            tyleht = "",
                            tylecc = 0
                        };
                        lsLV.Add(dMLV);
                        continue;
                    }

                    DateTime currentDate = new DateTime(date.Year, date.Month, day);

                    var dailyData = lsorder
                    .Where(x => x.fromDate.Date == currentDate)
                    .Select(x => (x.plannedquantity, x.shift_id))
                    .ToList();
                    if (dailyData.Count == 1)
                        checkca = true;

                    if (dailyData.Count > 0)
                    {
                        foreach (var dd in dailyData)
                        {
                            var timebdkt = getTimeCa.Where(x => x.id == dd.shift_id).ToList();
                            totalslca = lsorder.Where(x => x.fromDate.Date == currentDate && x.shift_id == dd.shift_id).FirstOrDefault().plannedquantity;
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
                                    ca = "S4";
                                    if ((timekt - timebd).TotalHours < 10)
                                        sumtimekh = 435;
                                    else
                                        sumtimekh = 615;
                                    if (checkca)
                                    {
                                        dMLV = new dataViewTable()
                                        {
                                            ngay = day,
                                            slplan = 0,
                                            slact = 0,
                                            calv = "HC",
                                            tyleht = "",
                                            tylecc = 0
                                        };
                                        lsLV.Add(dMLV);
                                    }
                                }
                                else
                                {
                                    ca = "HC";
                                    if ((timekt - timebd).TotalHours < 10)
                                        sumtimekh = 480;
                                    else
                                        sumtimekh = 660;
                                    if (checkca)
                                    {
                                        dMLV = new dataViewTable()
                                        {
                                            ngay = day,
                                            slplan = 0,
                                            slact = 0,
                                            calv = "S4",
                                            tyleht = "",
                                            tylecc = 0
                                        };
                                        lsLV.Add(dMLV);
                                    }
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
                                    if (listSLTrongNgay.LastOrDefault().quantity == 0)
                                    {
                                        slTTtrongngay = listSLTrongNgay[listSLTrongNgay.Count - 2].quantity;
                                    }
                                    else
                                    {
                                        slTTtrongngay = listSLTrongNgay.LastOrDefault().quantity;
                                    }
                            }

                            //act
                            if (ca == "S4")
                            {
                                dMactd = new DMPlan()
                                {
                                    ngay = day,
                                    gio = slTTtrongngay
                                };
                                lsactd.Add(dMactd);
                            }
                            else
                            {
                                dMacthc = new DMPlan
                                {
                                    ngay = day,
                                    gio = slTTtrongngay
                                };
                                lsacthc.Add(dMacthc);
                            }
                            
                            dMLV = new dataViewTable()
                            {
                                ngay = day,
                                slplan = totalslca,
                                slact = slTTtrongngay,
                                calv = ca
                            };
                            if (dMLV.slact >= dMLV.slplan)
                                dMLV.tyleht = "O";
                            else
                                dMLV.tyleht = "X";
                            dMLV.tylecc = Math.Round(((dMLV.slact * 35) / (sumtimekh*60)) * 100, 0);
                            lsLV.Add(dMLV);

                        }
                    }
                    else
                    {
                        dMLV = new dataViewTable()
                        {
                            ngay = day,
                            slplan = totalslca,
                            slact = slTTtrongngay,
                            calv = "S4",
                            tyleht = "",
                            tylecc = 0
                        };
                        lsLV.Add(dMLV);
                        dMLV = new dataViewTable()
                        {
                            ngay = day,
                            slplan = totalslca,
                            slact = slTTtrongngay,
                            calv = "HC",
                            tyleht = "",
                            tylecc = 0
                        };
                        lsLV.Add(dMLV);
                    }

                    
                }

                //show act dem
                if (lsactd.Count > 0)
                {
                    foreach (var it in lsactd)
                    {
                        actSeries.Points.AddXY(it.ngay, it.gio);
                        actSeries.ToolTip = (it.gio).ToString();
                    }
                }

                // show act hc
                if (lsacthc.Count > 0)
                {
                    foreach (var it in lsacthc)
                    {
                        planSeries.Points.AddXY(it.ngay, it.gio);
                        planSeries.ToolTip = (it.gio).ToString();
                    }
                }

                //show tyle
                tyleSeries.EmptyPointStyle.BorderDashStyle = ChartDashStyle.Dash;
                tyleSeries.EmptyPointStyle.MarkerStyle = MarkerStyle.Cross;
                tyleSeries.IsValueShownAsLabel = true;

                tylekhSeries.EmptyPointStyle.BorderDashStyle = ChartDashStyle.Dash;
                tylekhSeries.EmptyPointStyle.MarkerStyle = MarkerStyle.Cross;
                
                tylekhSeries.BorderDashStyle = ChartDashStyle.Dash;

                for (int day = 1; day <= 31; day++)
                {
                    var dscc = lsLV.Where(x => x.ngay == day).ToList();
                    DataPoint ttkh = new DataPoint();
                    ttkh.SetValueXY(day, 85);
                    tylekhSeries.Points.Add(ttkh);
                    tylekhSeries.IsValueShownAsLabel = false;

                    if (day == 31)
                    {
                        DataPoint lastPoint = tylekhSeries.Points[tylekhSeries.Points.Count - 1];
                        lastPoint.Label = "85%";
                    }

                    foreach (var item in dscc)
                    {
                        if (item.tylecc == 0)
                        {
                            DataPoint emptyPoint = new DataPoint(day, 0);
                            //emptyPoint.SetValueXY(day, 0);
                            emptyPoint.IsEmpty = true;
                            tyleSeries.Points.Add(emptyPoint);
                        }
                        else
                        {
                            DataPoint point = new DataPoint();
                            if(item.calv == "S4")
                                point.SetValueXY(day-0.3, item.tylecc);
                            else
                                point.SetValueXY(day+0.3, item.tylecc);
                            point.Label = $"{Math.Round(item.tylecc, 0)}%";
                            point.LabelForeColor = Color.Green;
                            tyleSeries.Points.Add(point);
                        }
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

            dgv.Font = new Font("Segoe UI", 7, FontStyle.Regular);
            dgv.Rows.Clear();
            dgv.Rows.Add(5);

            dgv.Rows[0].Cells[0].Value = "Ca";
            for (int i = 0; i< lsLV.Count; i++)
            {
                dgv.Rows[0].Cells[i+1].Value = lsLV[i].calv.ToString();
            }

            dgv.Rows[1].Cells[0].Value = "Sản lượng KH";
            for (int i = 0; i < lsLV.Count; i++)
            {
                dgv.Rows[1].Cells[i + 1].Value = lsLV[i].slplan.ToString();
            }
            dgv.Rows[1].Cells[63].Value = lsLV.Sum(x => x.slplan).ToString();

            dgv.Rows[2].Cells[0].Value = "Sản lượng TT";
            for (int i = 0; i < lsLV.Count; i++)
            {
                dgv.Rows[2].Cells[i + 1].Value = lsLV[i].slact.ToString();
            }
            dgv.Rows[2].Cells[63].Value = lsLV.Sum(x => x.slact).ToString();

            dgv.Rows[3].Cells[0].Value = "Tỉ lệ hoàn thành";
            for (int i = 0; i < lsLV.Count; i++)
            {
                dgv.Rows[3].Cells[i + 1].Value = lsLV[i].tyleht.ToString();
            }

            dgv.Rows[4].Cells[0].Value = "Tỉ lệ chạy chuyền";
            for (int i = 0; i < lsLV.Count; i++)
            {
                dgv.Rows[4].Cells[i + 1].Value = lsLV[i].tylecc.ToString() + "%";
            }
                        
        }

        private void dgv_SelectionChanged(object sender, EventArgs e)
        {
            dgv.ClearSelection();
            dgv.CurrentCell = null;
        }

        private void btnBackDay_Click(object sender, EventArgs e)
        {
            dtpSearch.Value = dtpSearch.Value.AddMonths(-1);
            loaddata(null, null);
        }

        private void btnNextDay_Click(object sender, EventArgs e)
        {
            dtpSearch.Value = dtpSearch.Value.AddMonths(1);
            loaddata(null, null);
        }
    }
}
