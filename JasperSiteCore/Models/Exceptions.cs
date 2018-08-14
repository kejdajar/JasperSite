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

    public class ThemeHelperException : Exception
    {
        private static string _message = "An error in ThemeHelper class occured.";
        public ThemeHelperException() : base(_message)
        {

        }
        public ThemeHelperException(string message)
            : base(message)
        {
        }

        public ThemeHelperException(Exception inner)
            : base(_message, inner)
        {
        }

        public ThemeHelperException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }

    public class DatabaseContextNullException : Exception
    {
        private static string _message = "Database context can't be null.";
        public DatabaseContextNullException() : base(_message)
        {

        }
        public DatabaseContextNullException(string message)
            : base(message)
        {
        }

        public DatabaseContextNullException(Exception inner)
            : base(_message, inner)
        {
        }

        public DatabaseContextNullException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }

    public class NotSupportedDatabaseException : Exception
    {
        private static string _message = "Selected database is not supported";
        public NotSupportedDatabaseException() : base(_message)
        {

        }
        public NotSupportedDatabaseException(string message)
            : base(message)
        {
        }

        public NotSupportedDatabaseException(Exception inner)
            : base(_message, inner)
        {
        }

        public NotSupportedDatabaseException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }

    public class DatabaseHelperException : Exception
    {
        private static string _message = "Database helper was unable to perform requested action.";
        public DatabaseHelperException() : base(_message)
        {

        }
        public DatabaseHelperException(string message)
            : base(message)
        {
        }

        public DatabaseHelperException(Exception inner)
            : base(_message, inner)
        {
        }

        public DatabaseHelperException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }

    public class CustomRoutingException : Exception
    {
        private static string _message = "Required view file was not found.";
        public CustomRoutingException() : base(_message)
        {

        }
        public CustomRoutingException(string message)
            : base(message)
        {
        }

        public CustomRoutingException(Exception inner)
            : base(_message, inner)
        {
        }

        public CustomRoutingException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }

    public class ThemeNotExistsException : Exception
    {
        private static string _message = "Jasper.json: activeTheme does not exist";
        public string MissingThemeName = string.Empty;
        public ThemeNotExistsException() : base(_message)
        {

        }
        public ThemeNotExistsException(string message)
            : base(message)
        {
        }

        public ThemeNotExistsException(Exception inner)
            : base(_message, inner)
        {
        }

        public ThemeNotExistsException(string message, Exception inner)
            : base(message, inner)
        {
        }

    }


}
