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

    public class GlobalConfigDataException : Exception
    {
        public GlobalConfigDataException() : base("Global configuration object can't be NULL")
        {

        }
      
    }

    public class GlobalConfigDataProviderException : Exception
    {
        private static string _message = "Json path parameter object can't be NULL";
        public GlobalConfigDataProviderException() : base(_message)
        {

        }
        public GlobalConfigDataProviderException(string message)
            : base(message)
        {
        }

        public GlobalConfigDataProviderException(Exception inner)
            : base(_message, inner)
        {
        }

        public GlobalConfigDataProviderException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }

    public class ConfigurationObjectProviderJsonException : Exception
    {
        private static string _message = "Global Website Config parameter object can't be NULL";
        public ConfigurationObjectProviderJsonException() : base(_message)
        {

        }
        public ConfigurationObjectProviderJsonException(string message)
            : base(message)
        {
        }

        public ConfigurationObjectProviderJsonException(Exception inner)
            : base(_message, inner)
        {
        }

        public ConfigurationObjectProviderJsonException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }

    public class ThemeConfigurationFileNotFoundException : Exception
    {
        private static string _message = "Theme configuration file was not found";
        public ThemeConfigurationFileNotFoundException() : base(_message)
        {

        }
        public ThemeConfigurationFileNotFoundException(string message)
            : base(message)
        {
        }

        public ThemeConfigurationFileNotFoundException(Exception inner)
            : base(_message, inner)
        {
        }

        public ThemeConfigurationFileNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }
}
