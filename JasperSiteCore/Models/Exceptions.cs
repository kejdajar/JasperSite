using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JasperSiteCore.Models
{

    public class ConfigurationObjectException : Exception
    {
        public ConfigurationObjectException():base("Configuration object can't be NULL")
        {
           
        }

        public ConfigurationObjectException(string message)
            : base(message)
        {
        }

        public ConfigurationObjectException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

}
