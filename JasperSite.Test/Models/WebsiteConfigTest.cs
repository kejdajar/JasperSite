using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using JasperSite.Controllers;
using Microsoft.AspNetCore.Mvc;
using JasperSite.Models;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Http;

namespace JasperSite.Test.Models
{
    [TestFixture]
    class WebsiteConfigTest
    {
        //[Test]      
        //public void WebsiteConfig_ParameterIsNull_ThrowsException()
        //{
        //    //// Arrange
        //    //ConfigurationObject configurationObject = null;                      

        //    //// Assert
        //    //Assert.That(() =>new WebsiteConfig(configurationObject), Throws.TypeOf<ConfigurationObjectException>());
        //}

        //[Test]
        //public void GetConfigData_ParameterIsNull_ThrowsException()
        //{
        //    //// Arrange
        //    //ConfigurationObject configurationObject = new ConfigurationObject();
        //    //WebsiteConfig websiteConfig = new WebsiteConfig(configurationObject);

        //    //// Acti
        //    //ConfigurationObject testResult = websiteConfig.GetConfigData();

        //    //// Assert
        //    //Assert.That(testResult, Is.SameAs(configurationObject));
        //}
    }
}
