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
    class CustomRoutingTest
    {
        GlobalConfigData S_GlobalConfigData;
        GlobalWebsiteConfig S_GlobalWebsiteConfig;
        [SetUp]
        public void Init()
        {
            S_GlobalConfigData = new GlobalConfigData()
            {
                themeName = "Jasper",
                themeFolder = "Themes"
            };

            S_GlobalWebsiteConfig = new GlobalWebsiteConfig(S_GlobalConfigData);
           
        }

        [Test]
        public void IsHomePage_UrlIsInHomePageArray_ReturnsTrue()
        {
            // Arrange
            string rawUrl = "/";
            string[] urlsInArray = new string[] { "/" };

            ConfigurationObject co = new ConfigurationObject()
            {
                routing = new ConfigurationObject.Routing() { homePage=urlsInArray}
            };
            WebsiteConfig websiteConfig = new WebsiteConfig(co);
            CustomRouting customRouting = new CustomRouting(websiteConfig, S_GlobalWebsiteConfig);

            // Act
            bool resultOfTest = customRouting.IsHomePage(rawUrl);

            // Assert
            // Raw Url is "/" and in homePage array is "/" on [0] so raw url is pointing to homepage
            Assert.IsTrue(resultOfTest);
        }

        [Test]
        public void IsHomePage_UrlIsNOTInHomePageArray_ReturnsFalse()
        {
            // Arrange
            string rawUrl = "/";
            string[] urlsInArray = new string[] { "/a/b/","\\","xyz" };

            ConfigurationObject co = new ConfigurationObject()
            {
                routing = new ConfigurationObject.Routing() { homePage = urlsInArray }
            };
            WebsiteConfig websiteConfig = new WebsiteConfig(co);
            CustomRouting customRouting = new CustomRouting(websiteConfig, S_GlobalWebsiteConfig);

            // Act
            bool resultOfTest = customRouting.IsHomePage(rawUrl);

            // Assert            
            Assert.IsFalse(resultOfTest);
        }





        [Test]
        public void GetHomePageUrls_UrlIsInHomePageArray_ReturnsThatArray()
        {
            // Arrange
            string[] urlsInArray = new string[] { "/","/Home/Index","/Home/Index/" };

            ConfigurationObject co = new ConfigurationObject()
            {
                routing = new ConfigurationObject.Routing() { homePage = urlsInArray }
            };
            WebsiteConfig websiteConfig = new WebsiteConfig(co);
            CustomRouting customRouting = new CustomRouting(websiteConfig, S_GlobalWebsiteConfig);

            // Act
            string[] resultOfTest = customRouting.GetHomePageUrls();

            // Assert           
            Assert.AreEqual(resultOfTest, urlsInArray);

            // Assert the same again (redundant)
            Assert.AreEqual(3, resultOfTest.Length);
            Assert.AreEqual(urlsInArray[0], resultOfTest[0]);
            Assert.AreEqual(urlsInArray[1], resultOfTest[1]);
            Assert.AreEqual(urlsInArray[2], resultOfTest[2]);
        }

        [Test]
        public void GetHomePageUrls_ArrayIsEmpty_ReturnsNULL()
        {
            // Arrange
            string[] urlsInArray = new string[] { };

            ConfigurationObject co = new ConfigurationObject()
            {
                routing = new ConfigurationObject.Routing() { homePage = urlsInArray }
            };
            WebsiteConfig websiteConfig = new WebsiteConfig(co);
            CustomRouting customRouting = new CustomRouting(websiteConfig, S_GlobalWebsiteConfig);

            // Act
            string[] resultOfTest = customRouting.GetHomePageUrls();

            // Assert           
            Assert.AreEqual(null,resultOfTest);

            
        }

    }
}
