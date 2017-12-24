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
//        GlobalConfigData S_GlobalConfigData;
//        GlobalWebsiteConfig S_GlobalWebsiteConfig;
//        [SetUp]
//        public void Init()
//        {
//            S_GlobalConfigData = new GlobalConfigData()
//            {
//                themeName = "ThemeName",
//                themeFolder = "ThemeFolder"
//            };

//            S_GlobalWebsiteConfig = new GlobalWebsiteConfig(S_GlobalConfigData);
           
//        }

//        [Test]
//        public void IsHomePage_UrlIsInHomePageArray_ReturnsTrue()
//        {
//            // Arrange
//            string rawUrl = "/";
//            string[] urlsInArray = new string[] { "/" };

//            ConfigurationObject co = new ConfigurationObject()
//            {
//                routing = new ConfigurationObject.Routing() { homePage=urlsInArray}
//            };
//            WebsiteConfig websiteConfig = new WebsiteConfig(co);
//            CustomRouting customRouting = new CustomRouting(websiteConfig, S_GlobalWebsiteConfig);

//            // Act
//            bool resultOfTest = customRouting.IsHomePage(rawUrl);

//            // Assert
//            // Raw Url is "/" and in homePage array is "/" on [0] so raw url is pointing to homepage
//            Assert.IsTrue(resultOfTest);
//        }

//        [Test]
//        public void IsHomePage_UrlIsNOTInHomePageArray_ReturnsFalse()
//        {
//            // Arrange
//            string rawUrl = "/";
//            string[] urlsInArray = new string[] { "/a/b/","\\","xyz" };

//            ConfigurationObject co = new ConfigurationObject()
//            {
//                routing = new ConfigurationObject.Routing() { homePage = urlsInArray }
//            };
//            WebsiteConfig websiteConfig = new WebsiteConfig(co);
//            CustomRouting customRouting = new CustomRouting(websiteConfig, S_GlobalWebsiteConfig);

//            // Act
//            bool resultOfTest = customRouting.IsHomePage(rawUrl);

//            // Assert            
//            Assert.IsFalse(resultOfTest);
//        }





//        [Test]
//        public void GetHomePageUrls_ArrayNotEmpty_ReturnsThatArray()
//        {
//            // Arrange
//            string[] urlsInArray = new string[] { "/","/Home/Index","/Home/Index/" };

//            ConfigurationObject co = new ConfigurationObject()
//            {
//                routing = new ConfigurationObject.Routing() { homePage = urlsInArray }
//            };
//            WebsiteConfig websiteConfig = new WebsiteConfig(co);
//            CustomRouting customRouting = new CustomRouting(websiteConfig, S_GlobalWebsiteConfig);

//            // Act
//            string[] resultOfTest = customRouting.GetHomePageUrls();

//            // Assert           
//            Assert.AreEqual(urlsInArray,resultOfTest);

//            // Assert the same again (redundant)
//            Assert.AreEqual(3, resultOfTest.Length);
//            Assert.AreEqual(urlsInArray[0], resultOfTest[0]);
//            Assert.AreEqual(urlsInArray[1], resultOfTest[1]);
//            Assert.AreEqual(urlsInArray[2], resultOfTest[2]);
//        }

//        [Test]
//        public void GetHomePageUrls_ArrayIsEmpty_ReturnsNULL()
//        {
//            // Arrange
//            string[] urlsInArray = new string[] { };

//            ConfigurationObject co = new ConfigurationObject()
//            {
//                routing = new ConfigurationObject.Routing() { homePage = urlsInArray }
//            };
//            WebsiteConfig websiteConfig = new WebsiteConfig(co);
//            CustomRouting customRouting = new CustomRouting(websiteConfig, S_GlobalWebsiteConfig);

//            // Act
//            string[] resultOfTest = customRouting.GetHomePageUrls();

//            // Assert           
//            Assert.AreEqual(null,resultOfTest);

            
//        }

//        [Test]
//        public void GetHomePageUrls_ArrayIsNull_ReturnsNULL()
//        {
//            // Arrange
//            string[] urlsInArray = null;

//            ConfigurationObject co = new ConfigurationObject()
//            {
//                routing = new ConfigurationObject.Routing() { homePage = urlsInArray }
//            };
//            WebsiteConfig websiteConfig = new WebsiteConfig(co);
//            CustomRouting customRouting = new CustomRouting(websiteConfig, S_GlobalWebsiteConfig);

//            // Act
//            string[] resultOfTest = customRouting.GetHomePageUrls();

//            // Assert           
//            Assert.AreEqual(null, resultOfTest);
            
//        }

//        [Test]
//        public void GetHomePageFile_Exists_ReturnsRootRelativeFile()
//        {
//            // Arrange
//            string homePageFileNameRelativeToFolder = "default.aspx";                     

//            GlobalWebsiteConfig globalWebsiteConfig = new GlobalWebsiteConfig(S_GlobalConfigData);

//            ConfigurationObject co = new ConfigurationObject()
//            {
//                routing = new ConfigurationObject.Routing() { homePageFile = homePageFileNameRelativeToFolder }
//            };
//            WebsiteConfig websiteConfig = new WebsiteConfig(co);
//            CustomRouting customRouting = new CustomRouting(websiteConfig, globalWebsiteConfig);

//            // Act
//            string resultOfTest = customRouting.GetHomePageFile();

//            // Assert
//            Assert.AreEqual("ThemeFolder/ThemeName/default.aspx", resultOfTest);
            
//        }

//#region RelativeThemePathToRootRelativePath
//        [Test]
//        public void RelativeThemePathToRootRelativePath_SameDirectory_ReturnsRootBasedPath()       
//        {
//            // Arrange            
//            string relativePath = "./file.txt";                       

//            GlobalWebsiteConfig globalWebsiteConfig = new GlobalWebsiteConfig(S_GlobalConfigData);

//            ConfigurationObject co = new ConfigurationObject()
//            {
//                routing = new ConfigurationObject.Routing() {  }
//            };
//            WebsiteConfig websiteConfig = new WebsiteConfig(co);
//            CustomRouting customRouting = new CustomRouting(websiteConfig, globalWebsiteConfig);

//            // Act
//            string resultOfTest = customRouting.RelativeThemePathToRootRelativePath(relativePath);

//            // Assert
//            Assert.AreEqual("ThemeFolder/ThemeName/file.txt", resultOfTest);

//        }

//        [Test]
//        public void RelativeThemePathToRootRelativePath_SubDirectory_ReturnsRootBasedPath()
//        {
//            // Arrange            
//            string relativePath = "./subdir/file.txt";                       

//            GlobalWebsiteConfig globalWebsiteConfig = new GlobalWebsiteConfig(S_GlobalConfigData);

//            ConfigurationObject co = new ConfigurationObject()
//            {
//                routing = new ConfigurationObject.Routing() { }
//            };
//            WebsiteConfig websiteConfig = new WebsiteConfig(co);
//            CustomRouting customRouting = new CustomRouting(websiteConfig, globalWebsiteConfig);

//            // Act
//            string resultOfTest = customRouting.RelativeThemePathToRootRelativePath(relativePath);

//            // Assert
//            Assert.AreEqual("ThemeFolder/ThemeName/subdir/file.txt", resultOfTest);
//        }

//        [Test]
//        public void RelativeThemePathToRootRelativePath_UpperDirectory_ReturnsRootBasedPath()
//        {
//            // Arrange            
//            string relativePath = "../upper/file.txt";

//            GlobalWebsiteConfig globalWebsiteConfig = new GlobalWebsiteConfig(S_GlobalConfigData);

//            ConfigurationObject co = new ConfigurationObject()
//            {
//                routing = new ConfigurationObject.Routing() { }
//            };
//            WebsiteConfig websiteConfig = new WebsiteConfig(co);
//            CustomRouting customRouting = new CustomRouting(websiteConfig, globalWebsiteConfig);

//            // Act
//            string resultOfTest = customRouting.RelativeThemePathToRootRelativePath(relativePath);

//            // Assert
//            Assert.AreEqual("ThemeFolder/upper/file.txt", resultOfTest);
//        }

//        [TestCase("")]
//        [TestCase(" ")]
//        [TestCase(null)]
//        public void RelativeThemePathToRootRelativePath_ParameterIsNullEmptyWhitespace_ReturnsNull(string param)
//        {
//            // Arrange            
//            string relativePath = param;

//            GlobalWebsiteConfig globalWebsiteConfig = new GlobalWebsiteConfig(S_GlobalConfigData);

//            ConfigurationObject co = new ConfigurationObject()
//            {
//                routing = new ConfigurationObject.Routing() { }
//            };
//            WebsiteConfig websiteConfig = new WebsiteConfig(co);
//            CustomRouting customRouting = new CustomRouting(websiteConfig, globalWebsiteConfig);

//            // Act
//            string resultOfTest = customRouting.RelativeThemePathToRootRelativePath(relativePath);

//            // Assert
//            Assert.AreEqual(null, resultOfTest);
//        }

//        #endregion

    }
}
