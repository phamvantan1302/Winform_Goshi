using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WinformGoshi.Models.dashboard.DashboardTTSX;
using System.Windows.Forms;
using WinformGoshi.Models.GSAppLibrary;
using WinformGoshi.Models.dashboard;

namespace WinformGoshi.Sevices.dashboard
{
    public class frmDashboardPTDungServices
    {
        public static List<DMStatusinfo> getDSStatusinfo1(DateTime fromdate, DateTime todate)
        {
            string sql = "";
            List<DMStatusinfo> ret = new List<DMStatusinfo>();
            DMStatusinfo item;
            try
            {
                sql = "select t0.id, t0.machinecode, t0.status, t0.created_at, t0.error_code, " +
                " t0.stoppagereason_id, t0.shifttimetableexception_id, t1.name, t2.name, " +
                " t2.fromdate, t2.todate " +
                " from productioncounting_iotstatusinfo t0" +
                " left join stoppage_stoppagereason t1 on t0.stoppagereason_id = t1.id " +
                " left join basic_shifttimetableexception t2 on t0.shifttimetableexception_id = t2.id" +
                " where t0.active = true " +
                " and t0.created_at >= '" + fromdate + "' " +
                " and t0.created_at <= '" + todate + "' ";
                //" and t0.machinecode = '" + mm + "' ";
                sql += " order by t0.created_at";
                using (var command = new NpgsqlCommand(sql, Globals.NpgsqlConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            item = new DMStatusinfo()
                            {
                                id = reader.GetInt32(0),
                                machinecode = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                status = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                created_at = reader.GetDateTime(3),
                                error_code = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                stoppagereason_id = reader.IsDBNull(5) ? -1 : reader.GetInt32(5),
                                shifttimetableexception_id = reader.IsDBNull(6) ? -1 : reader.GetInt32(6),
                                stop_name = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                shifttimetableexception_name = reader.IsDBNull(8) ? "" : reader.GetString(8),
                                fromdate = reader.IsDBNull(9) ? DateTime.Now : reader.GetDateTime(9),
                                todate = reader.IsDBNull(10) ? DateTime.Now : reader.GetDateTime(10),
                            };
                            ret.Add(item);
                        }
                        reader.Close();
                        command.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return ret;
        }

        public static List<DMStatusinfo> getDSStatusinfo2(DateTime date)
        {
            string sql = "";
            DateTime fromDate = new DateTime(date.Year, date.Month, 1);
            DateTime toDate = fromDate.AddMonths(1);
            List<DMStatusinfo> ret = new List<DMStatusinfo>();
            DMStatusinfo item;
            try
            {
                sql = "select t0.id, t0.machinecode, t0.status, t0.created_at, t0.error_code, " +
                " t0.stoppagereason_id, t0.shifttimetableexception_id, t1.name, t2.name, " +
                " t2.fromdate, t2.todate " +
                " from productioncounting_iotstatusinfo t0" +
                " left join stoppage_stoppagereason t1 on t0.stoppagereason_id = t1.id " +
                " left join basic_shifttimetableexception t2 on t0.shifttimetableexception_id = t2.id" +
                " where t0.active = true " +
                " and t0.created_at >= '" + fromDate.ToString("yyyy-MM-dd") + "' " +
                " and t0.created_at < '" + toDate.ToString("yyyy-MM-dd") + "' ";
                sql += " order by t0.created_at";
                using (var command = new NpgsqlCommand(sql, Globals.NpgsqlConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            item = new DMStatusinfo()
                            {
                                id = reader.GetInt32(0),
                                machinecode = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                status = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                created_at = reader.GetDateTime(3),
                                error_code = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                stoppagereason_id = reader.IsDBNull(5) ? -1 : reader.GetInt32(5),
                                shifttimetableexception_id = reader.IsDBNull(6) ? -1 : reader.GetInt32(6),
                                stop_name = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                shifttimetableexception_name = reader.IsDBNull(8) ? "" : reader.GetString(8),
                                fromdate = reader.IsDBNull(9) ? DateTime.Now : reader.GetDateTime(9),
                                todate = reader.IsDBNull(10) ? DateTime.Now : reader.GetDateTime(10),
                            };
                            ret.Add(item);
                        }
                        reader.Close();
                        command.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return ret;
        }

        public static List<DMOrder> getDataOrder(DateTime date, bool checkdate)
        {
            string sql = "";
            List<DMOrder> ret = new List<DMOrder>();
            DMOrder item;
            try
            {
                sql = "select t0.id, t0.plannedquantity, t0.startdate, t0.shift_id, t0.finishdate " +
                " from orders_order t0" +
                " where 1=1 ";
                if(checkdate)
                    sql += " and t0.finishdate::date = '" + date.ToString("yyyy-MM-dd") + "' ";
                sql += " order by t0.finishdate";
                using (var command = new NpgsqlCommand(sql, Globals.NpgsqlConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            item = new DMOrder()
                            {
                                id = reader.GetInt32(0),
                                plannedquantity = reader.IsDBNull(1) ? 0 : reader.GetDouble(1),
                                fromDate = reader.GetDateTime(2),
                                shift_id = reader.IsDBNull(3) ? -1 : reader.GetInt32(3),
                                toDate = reader.GetDateTime(4),
                            };
                            ret.Add(item);
                        }
                        reader.Close();
                        command.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return ret;
        }
    }
}
