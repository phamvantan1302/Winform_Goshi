using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinformGoshi.Models.GSAppLibrary;
using static WinformGoshi.Models.dashboard.DashboardTTSX;

namespace WinformGoshi.Sevices.dashboard
{
    public class frmDashboardTTSXServices
    {
        public static List<DMTimeByCa> getDataCaKH(DateTime date, int id)
        {
            string sql = "";
            List<DMTimeByCa> ret = new List<DMTimeByCa>();
            DateTime fromDate, toDate;

            // Các ca đêm: id 9, 10, 13, 14
            var nightShiftIds = new HashSet<int> { 9, 10, 13, 14 };

            if (nightShiftIds.Contains(id))
            {
                fromDate = date.AddDays(-1).Date.AddHours(18); 
                toDate = date.Date.AddHours(6);   
            }
            else
            {
                fromDate = date.Date.AddHours(6);        
                toDate = date.Date.AddHours(18);     
            }
            DMTimeByCa item;
            try
            {
                sql = " select t0.id, t0.name, t0.mondayhours, t0.tuesdayhours, t0.wensdayhours, t0.thursdayhours, " +
                    " t0.fridayhours, t0.saturdayhours, t0.sundayhours, t2.name, t2.fromdate, t2.todate, t2.type, t2.id " +
                    " from basic_shift t0 " +
                    " left join jointable_shift_shifttimetableexception t1 on t0.id = t1.shift_id " +
                    " left join basic_shifttimetableexception t2 on t1.shifttimetableexception_id = t2.id " +
                    " where t0.id = " + id;
                    sql += " and t2.fromdate >= '" + fromDate + "' " +
                    " and t2.fromdate <= '" + toDate + "' ";

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
                                iddung = reader.GetInt32(13),
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

        public static List<DMChuyen> getDSChuyen(DateTime date)
        {
            string sql = "";
            List<DMChuyen> ret = new List<DMChuyen>();
            DMChuyen item;
            try
            {
                sql = " select t1.id, t1.name " +
                    " from orders_order t0 " +
                    " left join productionlines_productionline t1 on t0.productionline_id = t1.id " +
                    " where t0.finishdate::date = '" + date.ToString("yyyy-MM-dd") + "' ";
                using (var command = new NpgsqlCommand(sql, Globals.NpgsqlConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            item = new DMChuyen()
                            {
                                id = reader.GetInt32(0),
                                name = reader.IsDBNull(1) ? "" : reader.GetString(1),
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

        public static List<DMModel> getDSModel(DateTime date, string idchuyen)
        {
            string sql = "";
            List<DMModel> ret = new List<DMModel>();
            DMModel item;
            try
            {
                sql = " select t2.id, t2.name " +
                    " from orders_order t0 " +
                    " left join basic_product t1 on t0.product_id = t1.id " +
                    " left join basic_model t2 on t1.model_id = t2.id " +
                    " where t0.finishdate::date = '" + date.ToString("yyyy-MM-dd") + "' " +
                    " and t0.productionline_id = " + idchuyen ;
                using (var command = new NpgsqlCommand(sql, Globals.NpgsqlConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            item = new DMModel()
                            {
                                id = reader.GetInt32(0),
                                name = reader.IsDBNull(1) ? "" : reader.GetString(1),
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

        public static List<DMShift> getIdCa(DateTime date, string idchuyen)
        {
            string sql = "";
            List<DMShift> ret = new List<DMShift>();
            DMShift item = null;
            try
            {
                sql = " select t0.shift_id, t1.name " +
                    " from orders_order t0 " +
                    " left join basic_shift t1 on t0.shift_id = t1.id " +
                    " where t0.finishdate::date = '" + date.ToString("yyyy-MM-dd") + "' and t0.productionline_id = " + idchuyen;
                using (var command = new NpgsqlCommand(sql, Globals.NpgsqlConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            item = new DMShift()
                            {
                                id = reader.GetInt32(0),
                                name = reader.IsDBNull(1) ? "" : reader.GetString(1),
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

        public static double getSLKH(DateTime date, string idchuyen, int idca)
        {
            string sql = "";
            double ret = 0;
            try
            {
                sql = " select t0.plannedquantity " +
                    " from orders_order t0 " +
                    " where t0.finishdate::date = '" + date.ToString("yyyy-MM-dd") + "' " +
                    " and t0.productionline_id = " + idchuyen +
                    " and t0.shift_id = " + idca;
                using (var command = new NpgsqlCommand(sql, Globals.NpgsqlConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ret = reader.IsDBNull(0) ? 0 : reader.GetDouble(0);
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

        public static string getMMTB(DateTime date, string line, string mode)
        {
            string sql = "";
            string ret = "";
            try
            {
                sql = " select t3.number " +
                    " from orders_order t0 " +
                    " left join productionlines_productionline t1 on t0.productionline_id = t1.id " +
                    " left join basic_product t2 on t0.product_id = t2.id " +
                    " left join basic_workstation t3 on t0.workstation_id = t3.id" +
                    " where t0.finishdate::date = '" + date + "' " +
                    " and t0.productionline_id = "+line +
                    " and t2.model_id = "+ mode;

                using (var command = new NpgsqlCommand(sql, Globals.NpgsqlConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ret = reader.IsDBNull(0) ? "" : reader.GetString(0);
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

        public static List<DMCounterinfo> getDSCounterinfo(DateTime fromdate, DateTime todate, string mm)
        {
            string sql = "";
            List<DMCounterinfo> ret = new List<DMCounterinfo>();
            DMCounterinfo item;
            try
            {
                sql = "select id, machinecode, quantity, counter, created_at " +
                      " from productioncounting_counterinfo " +
                      " where created_at >= (" +
                      "  select max(created_at) " +
                      "  from productioncounting_counterinfo " +
                      "  where machinecode = '" + mm + "' " +
                      "  and quantity = 1 " +
                      "  and created_at >= '" + fromdate.AddMinutes(-20) + "' " +
                      "  and created_at <= '" + fromdate.AddHours(1) + "'" +
                      ") " +
                      " and created_at <= '" + todate + "' " +
                      " and machinecode = '" + mm + "'";
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

        public static List<DMStatusinfo> getDSStatusinfo(DateTime fromdate, DateTime todate, string mm)
        {
            string sql = "";
            List <DMStatusinfo> ret = new List<DMStatusinfo>();
            DMStatusinfo item;
            try
            {
                sql = "select t0.id, t0.machinecode, t0.status, t0.created_at, t0.error_code, " +
                " t0.stoppagereason_id, t0.shifttimetableexception_id, t1.name, t2.name, " +
                " t2.fromdate, t2.todate, t0.note " +
                " from productioncounting_iotstatusinfo t0" +
                " left join stoppage_stoppagereason t1 on t0.stoppagereason_id = t1.id " +
                " left join basic_shifttimetableexception t2 on t0.shifttimetableexception_id = t2.id" +
                " where t0.active = true " +
                " and t0.created_at >= '" + fromdate + "' " +
                " and t0.created_at <= '" + todate + "' " +
                " and t0.machinecode = '" + mm + "' ";
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
                                note = reader.IsDBNull(11) ? "" : reader.GetString(11),
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

        public static string getStatusinfo(string mm)
        {
            string sql = "";
            string ret = "";
            try
            {
                sql = " select id, machinecode, status, created_at, active " +
                    " from productioncounting_iotstatusinfo " +
                    " where active = true " +
                    " and machinecode = '" + mm + "' " +
                    " order by created_at desc " +
                    " limit 1";
                using (var command = new NpgsqlCommand(sql, Globals.NpgsqlConnection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ret = reader.IsDBNull(2) ? "" : reader.GetString(2);
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
