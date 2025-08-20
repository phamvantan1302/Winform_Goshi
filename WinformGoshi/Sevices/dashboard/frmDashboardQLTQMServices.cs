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
    public class frmDashboardQLTQMServices
    {
        public static List<DMTimeByCa> getDataCaKH(DateTime date)
        {
            string sql = "";
            DateTime fromDate = new DateTime(date.Year, date.Month, 1);
            DateTime toDate = fromDate.AddMonths(1);
            List<DMTimeByCa> ret = new List<DMTimeByCa>();
            DMTimeByCa item;
            try
            {
                sql = " select t0.id, t0.name, t0.mondayhours, t0.tuesdayhours, t0.wensdayhours, t0.thursdayhours, " +
                    " t0.fridayhours, t0.saturdayhours, t0.sundayhours, t2.name, t2.fromdate, t2.todate, t2.type " +
                    " from basic_shift t0 " +
                    " left join jointable_shift_shifttimetableexception t1 on t0.id = t1.shift_id " +
                    " left join basic_shifttimetableexception t2 on t1.shifttimetableexception_id = t2.id " +
                    " where t2.fromdate >= '" + fromDate.ToString("yyyy-MM-dd") + "' " +
                    " and t2.fromdate < '" + toDate.ToString("yyyy-MM-dd") + "' ";
                sql += " order by t2.fromdate";
                using (var command = new NpgsqlCommand(sql, Globals.NpgsqlConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            item = new DMTimeByCa()
                            {
                                id = reader.GetInt32(0),
                                name = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                mondayhours = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                tuesdayhours = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                wednesdayhours = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                thursdayhours = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                fridayhours = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                saturdayhours = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                sundayhours = reader.IsDBNull(8) ? "" : reader.GetString(8),
                                namesc = reader.IsDBNull(9) ? "" : reader.GetString(9),
                                fromdate = reader.GetDateTime(10),
                                todate = reader.GetDateTime(11),
                                type = reader.IsDBNull(12) ? "" : reader.GetString(12),
                            };
                            ret.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return ret;
        }

        public static List<DMStatusinfo> getDSStatusinfo(DateTime date)
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
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return ret;
        }

        public static List<DMCounterinfo> getDSCounterinfo(DateTime date)
        {
            string sql = "";
            DateTime fromDate = new DateTime(date.Year, date.Month, 1);
            DateTime toDate = fromDate.AddMonths(1);
            List<DMCounterinfo> ret = new List<DMCounterinfo>();
            DMCounterinfo item;
            try
            {
                sql = "select id, machinecode, quantity, counter, created_at " +
                      " from productioncounting_counterinfo " +
                      " where created_at >= (" +
                      " select min(created_at) " +
                      " from productioncounting_counterinfo " +
                      " where 1=1 " +
                      " and quantity = 1 " +
                      " and created_at >= '" + fromDate.ToString("yyyy-MM-dd 06:00:00") + "' " +
                      " and created_at < '" + toDate.ToString("yyyy-MM-dd 06:00:00") + "' " +
                      ") " +
                      " and created_at <= '" + toDate.ToString("yyyy-MM-dd 06:00:00") + "' ";
                sql += " order by created_at ";
                using (var command = new NpgsqlCommand(sql, Globals.NpgsqlConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            item = new DMCounterinfo()
                            {
                                id = reader.GetInt32(0),
                                machinecode = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                quantity = reader.GetDouble(2),
                                counter = reader.GetDouble(3),
                                created_at = reader.GetDateTime(4),
                            };
                            ret.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return ret;
        }

        public static List<DMCounterinfo> getSLByNgay(DateTime fromDate, DateTime toDate)
        {
            string sql = "";
            List<DMCounterinfo> ret = new List<DMCounterinfo>();
            DMCounterinfo item;
            try
            {
                sql = "select id, machinecode, quantity, counter, created_at " +
                      " from productioncounting_counterinfo " +
                      " where created_at >= (" +
                      " select max(created_at) " +
                      " from productioncounting_counterinfo " +
                      " where 1=1 " +
                      " and quantity = 1 " +
                      " and created_at >= '" + fromDate.AddMinutes(-20).ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                      " and created_at < '" + fromDate.AddHours(1).ToString("yyyy-MM-dd HH:mm:ss") + "' " +
                      ") " +
                      " and created_at <= '" + toDate.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
                sql += " order by created_at ";
                using (var command = new NpgsqlCommand(sql, Globals.NpgsqlConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            item = new DMCounterinfo()
                            {
                                id = reader.GetInt32(0),
                                machinecode = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                quantity = reader.GetDouble(2),
                                counter = reader.GetDouble(3),
                                created_at = reader.GetDateTime(4),
                            };
                            ret.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return ret;
        }

        public static List<DMOrder> getDataOrder(DateTime date)
        {
            string sql = "";
            DateTime fromDate = new DateTime(date.Year, date.Month, 1);
            DateTime toDate = fromDate.AddMonths(1);
            List<DMOrder> ret = new List<DMOrder>();
            DMOrder item;
            try
            {
                sql = "select t0.id, t0.plannedquantity, t0.finishdate, t0.shift_id " +
                " from orders_order t0" +
                " where t0.finishdate >= '" + fromDate.ToString("yyyy-MM-dd") + "' " +
                " and t0.finishdate < '" + toDate.ToString("yyyy-MM-dd") + "' ";
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
                            };
                            ret.Add(item);
                        }
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
