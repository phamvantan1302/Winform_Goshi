using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using WinformGoshi.Models.GSAppLibrary;

namespace WinformGoshi.Models.GSAppLibrary
{
    public class Globals
    {
        private static string _MESDBConnectionString = "";
        private static readonly AppConfigs _appConfigs = ConfigurationManager.GetSection("AppConfigs") as AppConfigs;

        public static string MESDBConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_MESDBConnectionString))
                {
                    _MESDBConnectionString += "Server=" + _appConfigs.MESDBConfigs.Server + ";";
                    _MESDBConnectionString += "Port=" + _appConfigs.MESDBConfigs.Port + ";";
                    _MESDBConnectionString += "User Id=" + _appConfigs.MESDBConfigs.UserId + ";";
                    _MESDBConnectionString += "Password=" + _appConfigs.MESDBConfigs.Password + ";";
                    _MESDBConnectionString += "Database=" + _appConfigs.MESDBConfigs.Database + ";";
                }
                return _MESDBConnectionString;
            }
        }

        //public static ProductionWarehouseConfigs productionWarehouseConfigs
        //{
        //    get
        //    {
        //        return _appConfigs.ProductionWarehouseConfigs;
        //    }
        //}

        private static NpgsqlConnection _conn = null;
        public static NpgsqlConnection NpgsqlConnection
        {
            get
            {
                if (_conn == null)
                {
                    _conn = new NpgsqlConnection(MESDBConnectionString);
                }

                if (_conn.State != System.Data.ConnectionState.Open)
                    _conn.Open();

                return _conn;
            }
        }

        public static FileServerConfig FileServerConfig
        {
            get
            {
                return _appConfigs.FileServerConfig;
            }
        }

        public static ExcelTemplatePath ExcelTemplatePath
        {
            get
            {
                return _appConfigs.ExcelTemplatePath;
            }
        }
    }
}
