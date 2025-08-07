using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformGoshi.Models.GSAppLibrary
{
    public class AppConfigs : ConfigurationSection
    {
        [ConfigurationProperty("MESDBConfigs")]
        public MESDBConfigs MESDBConfigs
        {
            get
            {
                return (MESDBConfigs)this["MESDBConfigs"];
            }
            set
            {
                value = (MESDBConfigs)this["MESDBConfigs"];
            }
        }

        //[ConfigurationProperty("ProductionWarehouseConfigs")]
        //public ProductionWarehouseConfigs ProductionWarehouseConfigs
        //{
        //    get
        //    {
        //        return (ProductionWarehouseConfigs)this["ProductionWarehouseConfigs"];
        //    }
        //    set
        //    {
        //        value = (ProductionWarehouseConfigs)this["ProductionWarehouseConfigs"];
        //    }
        //}
        [ConfigurationProperty("FileServerConfig")]
        public FileServerConfig FileServerConfig
        {
            get
            {
                return (FileServerConfig)this["FileServerConfig"];
            }
            set
            {
                value = (FileServerConfig)this["FileServerConfig"];
            }
        }
        [ConfigurationProperty("ExcelTemplatePath")]
        public ExcelTemplatePath ExcelTemplatePath
        {
            get
            {
                return (ExcelTemplatePath)this["ExcelTemplatePath"];
            }
            set
            {
                value = (ExcelTemplatePath)this["ExcelTemplatePath"];
            }
        }
    }
}
