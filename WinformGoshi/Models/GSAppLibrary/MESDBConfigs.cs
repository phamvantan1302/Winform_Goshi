using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinformGoshi.Models.GSAppLibrary
{
    public class MESDBConfigs : ConfigurationElement
    {
        [ConfigurationProperty("Server", DefaultValue = "10.1.1.21", IsRequired = true)]
        public string Server
        {
            get
            {
                return (string)this["Server"];
            }
            set
            {
                value = (string)this["Server"];
            }
        }
        [ConfigurationProperty("Port", DefaultValue = "5432", IsRequired = true)]
        public string Port
        {
            get
            {
                return (string)this["Port"];
            }
            set
            {
                value = (string)this["Port"];
            }
        }
        [ConfigurationProperty("UserId", DefaultValue = "postgres", IsRequired = true)]
        public string UserId
        {
            get
            {
                return (string)this["UserId"];
            }
            set
            {
                value = (string)this["UserId"];
            }
        }
        [ConfigurationProperty("Password", DefaultValue = "postgres123", IsRequired = true)]
        public string Password
        {
            get
            {
                return (string)this["Password"];
            }
            set
            {
                value = (string)this["Password"];
            }
        }
        [ConfigurationProperty("Database", DefaultValue = "mes", IsRequired = true)]
        public string Database
        {
            get
            {
                return (string)this["Database"];
            }
            set
            {
                value = (string)this["Database"];
            }
        }
    }
}
