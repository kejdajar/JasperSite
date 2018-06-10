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
        public void DeleteCategory()
        {
            // Arrange
          List<Category> categories = new List<Category>() {
                new Category(){Id=1,Name="Nezařazeno"},
                 new Category(){Id=2,Name="Programování"},
                  new Category(){Id=3,Name="Zábava"},
            };
            IQueryable<Category> iqueryableList = categories.AsQueryable();
            var mockSetCategories = new Mock<DbSet<Category>>(); 
           
            mockSetCategories.As<IQueryable<Category>>().Setup(e => e.Provider).Returns(iqueryableList.Provider);
            mockSetCategories.As<IQueryable<Category>>().Setup(e => e.Expression).Returns(iqueryableList.Expression);
            mockSetCategories.As<IQueryable<Category>>().Setup(e => e.ElementType).Returns(iqueryableList.ElementType);
            mockSetCategories.As<IQueryable<Category>>().Setup(e => e.GetEnumerator()).Returns(iqueryableList.GetEnumerator());
                   

            Mock<IDatabaseContext> mockContext = new Mock<IDatabaseContext>();         
            mockContext.Setup(e=>e.Categories).Returns(mockSetCategories.Object);

            // Add, remove

            //.Callback<Category>((entity) => categories.Remove(entity));
            //mockContext.Setup(m => m.Categories.Add(It.IsAny<Category>())).Callback((Category category1) => categories.Add(category1));

            // Removal
            // Removal specification !
            mockSetCategories.Setup(d => d.Remove(It.IsAny<Category>())).Callback<Category>(s => categories.Remove(s));
            Category first = mockContext.Object.Categories.FirstOrDefault();
            mockContext.Object.Categories.Remove(first);
            Category second = mockContext.Object.Categories.FirstOrDefault();
            mockContext.Object.Categories.Remove(second);

            // WORKING !!!!!
            int count = mockSetCategories.Object.Count();

            // WORKING !!!
          int count1= mockContext.Object.Categories.Count();
          

            

            ;
        }

        //[Test]
        //public void DbHelper_ParameterNull_ThrowsException()
        //{
        //    DatabaseContext context = null;
        //    Assert.That(() => new DbHelper(context), Throws.Exception.TypeOf<DatabaseContextNullException>());
        //}

        //[Test]
        //public void GetAllArticles_NoneArticle_ReturnsNull()
        //{
        //    IQueryable<Article> articles = new List<Article>().AsQueryable();
        //    var mockSet = new Mock<DbSet<Article>>();
        //    mockSet.As<IQueryable<Article>>().Setup(e => e.Provider).Returns(articles.Provider);
        //    mockSet.As<IQueryable<Article>>().Setup(e => e.Expression).Returns(articles.Expression);
        //    mockSet.As<IQueryable<Article>>().Setup(e => e.ElementType).Returns(articles.ElementType);
        //    mockSet.As<IQueryable<Article>>().Setup(e => e.GetEnumerator()).Returns(articles.GetEnumerator());
        //    var mockContext = new Mock<IDatabaseContext>();
        //    mockContext.Setup(e => e.Articles).Returns(mockSet.Object);

        //    DbHelper helper = new DbHelper(mockContext.Object);
        //    List<Article> testResult = helper.GetAllArticles();

        //    Assert.IsNull(testResult);
        //}

        //[Test]
        //public void GetAllArticles_ArticlesExists_ReturnsArticles()
        //{
        //    IQueryable<Article> articles = new List<Article>()
        //    {
        //    new Article() { Id=1,HtmlContent="test1", Name="article1", PublishDate=DateTime.Now},
        //    new Article() { Id=2,HtmlContent="test2", Name="article2", PublishDate=DateTime.Now}
        //    }
        //    .AsQueryable();

        //    var mockSet = new Mock<DbSet<Article>>();
        //    mockSet.As<IQueryable<Article>>().Setup(e => e.Provider).Returns(articles.Provider);
        //    mockSet.As<IQueryable<Article>>().Setup(e => e.Expression).Returns(articles.Expression);
        //    mockSet.As<IQueryable<Article>>().Setup(e => e.ElementType).Returns(articles.ElementType);
        //    mockSet.As<IQueryable<Article>>().Setup(e => e.GetEnumerator()).Returns(articles.GetEnumerator());
        //    var mockContext = new Mock<IDatabaseContext>();
        //    mockContext.Setup(e => e.Articles).Returns(mockSet.Object);

        //    DbHelper helper = new DbHelper(mockContext.Object);
        //    List<Article> testResult = helper.GetAllArticles();

        //    Assert.AreEqual(2, testResult.Count);
        //}

    }
}
