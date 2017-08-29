using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using JasperSiteCore.Models;
using JasperSiteCore.Models.Providers;
using JasperSiteCore.Models.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace JasperSiteCore.Test.Models.Database
{
    [TestFixture]
    class DbHelperTest
    {
        [Test]
        public void DbHelper_ParameterNull_ThrowsException()
        {
            DatabaseContext context = null;
            Assert.That(() => new DbHelper(context), Throws.Exception.TypeOf<DatabaseContextNullException>());
        }

        [Test]
        public void GetAllArticles_NoneArticle_ReturnsNull()
        {
            IQueryable<Article> articles = new List<Article>().AsQueryable();
            var mockSet = new Mock<DbSet<Article>>();
            mockSet.As<IQueryable<Article>>().Setup(e => e.Provider).Returns(articles.Provider);
            mockSet.As<IQueryable<Article>>().Setup(e => e.Expression).Returns(articles.Expression);
            mockSet.As<IQueryable<Article>>().Setup(e => e.ElementType).Returns(articles.ElementType);
            mockSet.As<IQueryable<Article>>().Setup(e => e.GetEnumerator()).Returns(articles.GetEnumerator());
            var mockContext = new Mock<IDatabaseContext>();
            mockContext.Setup(e => e.Articles).Returns(mockSet.Object);

            DbHelper helper = new DbHelper(mockContext.Object);
            List<Article> testResult = helper.GetAllArticles();

            Assert.IsNull(testResult);
        }

        [Test]
        public void GetAllArticles_ArticlesExists_ReturnsArticles()
        {
            IQueryable<Article> articles = new List<Article>()
            {
            new Article() { Id=1,HtmlContent="test1", Name="article1", PublishDate=DateTime.Now},
            new Article() { Id=2,HtmlContent="test2", Name="article2", PublishDate=DateTime.Now}
            }
            .AsQueryable();

            var mockSet = new Mock<DbSet<Article>>();
            mockSet.As<IQueryable<Article>>().Setup(e => e.Provider).Returns(articles.Provider);
            mockSet.As<IQueryable<Article>>().Setup(e => e.Expression).Returns(articles.Expression);
            mockSet.As<IQueryable<Article>>().Setup(e => e.ElementType).Returns(articles.ElementType);
            mockSet.As<IQueryable<Article>>().Setup(e => e.GetEnumerator()).Returns(articles.GetEnumerator());
            var mockContext = new Mock<IDatabaseContext>();
            mockContext.Setup(e => e.Articles).Returns(mockSet.Object);

            DbHelper helper = new DbHelper(mockContext.Object);
            List<Article> testResult = helper.GetAllArticles();

            Assert.AreEqual(2, testResult.Count);
        }

    }
}
