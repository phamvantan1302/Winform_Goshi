using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformGoshi.Models.GSAppLibrary
{
    public class FileServerConfig : ConfigurationElement
    {
        //[ConfigurationProperty("path", DefaultValue = "\\\\10.1.1.21\\mes_files", IsRequired = true)]
        [ConfigurationProperty("path", DefaultValue = "\\\\210.245.34.58\\mes_files", IsRequired = true)]
        public string path
        {
            get
            {
                return (string)this["path"];
            }
            set
            {
                value = (string)this["path"];
            }
        }
        [ConfigurationProperty("defectfolder", DefaultValue = "defect", IsRequired = true)]
        public string defectfolder
        {
            get
            {
                return (string)this["defectfolder"];
            }
            set
            {
                value = (string)this["defectfolder"];
            }
        }
        //[ConfigurationProperty("domain", DefaultValue = "\\10.1.1.21", IsRequired = true)]
        [ConfigurationProperty("domain", DefaultValue = "\\210.245.34.58:13581", IsRequired = true)]
        public string domain
        {
            get
            {
                return (string)this["domain"];
            }
            set
            {
                value = (string)this["domain"];
            }
        }
        [ConfigurationProperty("username", DefaultValue = "fti", IsRequired = true)]
        public string username
        {
            get
            {
                return (string)this["username"];
            }
            set
            {
                value = (string)this["username"];
            }
        }
        //[ConfigurationProperty("password", DefaultValue = "HKMes@159", IsRequired = true)]
        [ConfigurationProperty("password", DefaultValue = "HongKy!@2024", IsRequired = true)]
        public string password
        {
            get
            {
                return (string)this["password"];
            }
            set
            {
                value = (string)this["password"];
            }
        }
    }
}
