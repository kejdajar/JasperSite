using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using JasperSiteCore.Models;
using JasperSiteCore.Models.Providers;

namespace JasperSiteCore.Test.Providers
{
    [TestFixture]
    class ConfigurationObjectProviderJsonTest
    {
        GlobalWebsiteConfig S_GlobalWebsiteConfig;
        [SetUp]
        public void Setup()
        {
            GlobalConfigData gcd = new GlobalConfigData() { themeName = "ThemeName", themeFolder = "ThemeFolder" };
            S_GlobalWebsiteConfig = new GlobalWebsiteConfig(gcd);
        }

        [Test]
        public void ConfigurationObjectProviderJson_ParameterIsNull_ThrowsException()
        {
            GlobalWebsiteConfig globalWebsiteConfig = null;

            Assert.That(() => new ConfigurationObjectProviderJson(globalWebsiteConfig), Throws.Exception.TypeOf<ConfigurationObjectProviderJsonException>());
            
        }

        [Test]
        public void GetConfigData_FileNotExist_ThrowsException()
        {
            string notExistingFile = System.Guid.NewGuid().ToString();
            ConfigurationObjectProviderJson copj = new ConfigurationObjectProviderJson(S_GlobalWebsiteConfig,notExistingFile);
            Assert.That(() => copj.GetConfigData(), Throws.Exception.TypeOf<ConfigurationObjectProviderJsonException>());

        }

        [Test]
        public void GetThemeJasperJsonLocation_FileNotFound_ThrowsError()
        {
            // Arrange
            string notExistingFile = System.Guid.NewGuid().ToString() + ".json";
            ConfigurationObjectProviderJson copj = new ConfigurationObjectProviderJson(S_GlobalWebsiteConfig,notExistingFile);
            
            // Assert
            Assert.That(() => copj.GetThemeJasperJsonLocation(), Throws.Exception.TypeOf<ThemeConfigurationFileNotFoundException>());
        }
    }
}
