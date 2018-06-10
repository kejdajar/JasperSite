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
    class GlobalConfigDataProviderJsonTest
    {
        //[TestCase("")]
        //[TestCase(" ")]
        //[TestCase(null)]
        //public void GlobalConfigDataProviderJson_ParameterNullEmptyWhiteSpace_ThrowsException(string param)
        //{
        //    string jsonFilePath = param;
            
        //    Assert.That(()=> new GlobalConfigDataProviderJson(jsonFilePath), Throws.TypeOf<GlobalConfigDataProviderException>());
        //}

        //[Test]
        //public void GetGlobalConfigData_FileNotExists_ThrowsException()
        //{
        //    //string notExistingFile= Guid.NewGuid().ToString();
        //    //GlobalConfigDataProviderJson gcdpj = new GlobalConfigDataProviderJson(notExistingFile);

        //    //Assert.That(() => gcdpj.GetGlobalConfigData(), Throws.Exception.TypeOf<GlobalConfigDataProviderException>());
        //}

    }
}
