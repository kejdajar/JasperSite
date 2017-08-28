using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using JasperSiteCore.Models;
using JasperSiteCore.Models.Providers;

namespace JasperSiteCore.Test.Models
{
    [TestFixture]
    class GlobalWebsiteConfigTest
    {
        [Test]
        public void GlobalWebsiteConfig_ParameterNull_ThrowsException()
        {
            // Arrange
            GlobalConfigData globalConfigData = null;            

            // Act, Assert
            Assert.That(()=>  new GlobalWebsiteConfig(globalConfigData),Throws.TypeOf<GlobalConfigDataException>());
        }
    }
}
