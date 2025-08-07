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
            tRefreshData.Tick += new EventHandler(loaddashboard);
            tRefreshData.Interval = 10000;
            tRefreshData.Enabled = true;
            loaddashboard(null, null);
        }

        private void loaddashboard(object sender, EventArgs e)
        {
            if (isFormClosing || chart1 == null || this.IsDisposed)
                return;
            double tongerror = 0, sumdung = 0;
            List<DMStatusinfo> lsstatus = frmDashboardPTDungServices.getDSStatusinfo(DateTime.Now);
            if (lsstatus.Count > 0)
            {
                // dataview chart 1
                List<DMViewPie> lschart1 = new List<DMViewPie>();
                DMViewPie rcchart1 = new DMViewPie();

                for (int i = 0; i < lsstatus.Count; i++)
                {
                    if (lsstatus[i].status == "MAY_LOI")
                    {
                        rcchart1 = new DMViewPie()
                        {
                            name = lsstatus[i].stop_name
                        };
                        if (i + 1 == lsstatus.Count)
                        {
                            rcchart1.slerror = (DateTime.Now - lsstatus[i].created_at).TotalMinutes;
                        }
                        else
                        {
                            for (int j = i+1; j< lsstatus.Count; j++)
                            {
                                if (lsstatus[j].status != "MAY_LOI")
                                {
                                    rcchart1.slerror = (lsstatus[j].created_at - lsstatus[i].created_at).TotalMinutes;
                                    break;
                                }
                            }
                            
                        }
                        tongerror = rcchart1.slerror + tongerror;
                        lschart1.Add(rcchart1);

                    }
                }

                var groupedList = lschart1
                        .GroupBy(x => x.name)
                        .Select(g => new DMViewPie
                        {
                            name = g.Key,
                            slerror = g.Sum(x => x.slerror)
                        })
                        .ToList();

                // dataview chart 2
                List<DMViewPie> lschart2 = new List<DMViewPie>();
                DMViewPie rcchart2 = new DMViewPie();
                for (int i = 0; i < lsstatus.Count; i++)
                {
                    if (lsstatus[i].status == "MAY_DUNG")
                    {
                        rcchart2 = new DMViewPie()
                        {
                            name = lsstatus[i].shifttimetableexception_name
                        };
                        if (i + 1 == lsstatus.Count)
                        {
                            rcchart2.slerror = (DateTime.Now - lsstatus[i].created_at).TotalMinutes;
                        }
                        else
                        {
                            rcchart2.slerror = (lsstatus[i + 1].created_at - lsstatus[i].created_at).TotalMinutes;
                        }
                        sumdung = rcchart2.slerror + sumdung;
                        lschart2.Add(rcchart2);
                    }
                }
                var groupedList1 = lschart2
                        .GroupBy(x => x.name)
                        .Select(g => new DMViewPie
                        {
                            name = g.Key,
                            slerror = g.Sum(x => x.slerror)
                        })
                        .ToList();

                Series pieSeriesFM = chart1.Series["Series1"];
                pieSeriesFM.Points.Clear();
                Series pieSeriesDung = chart2.Series["Series1"];
                pieSeriesDung.Points.Clear();

                //chart 1
                foreach (var item in groupedList)
                {
                    int pointIndex = pieSeriesFM.Points.AddXY(item.name, Math.Round((double.Parse(item.slerror.ToString()) / tongerror) * 100, 0));
                    pieSeriesFM.Points[pointIndex].Label = $"{Math.Round((double.Parse(item.slerror.ToString()) / tongerror) * 100, 0)}%";
                    pieSeriesFM.Points[pointIndex].LegendText = item.name;
                }
                pieSeriesFM.IsValueShownAsLabel = true;
                pieSeriesFM.Font = new Font("Arial", 12, FontStyle.Regular);

                //chart 2
                foreach (var item in groupedList1)
                {
                    int pointIndex = pieSeriesDung.Points.AddXY(item.name, Math.Round((double.Parse(item.slerror.ToString()) / sumdung) * 100, 0));
                    pieSeriesDung.Points[pointIndex].Label = $"{Math.Round((double.Parse(item.slerror.ToString()) / sumdung) * 100, 0)}%";
                    pieSeriesDung.Points[pointIndex].LegendText = item.name;
                }
                pieSeriesDung.IsValueShownAsLabel = true;
                pieSeriesDung.Font = new Font("Arial", 12, FontStyle.Regular); 
            }

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
    }
}
