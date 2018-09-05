using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using JasperSite.Models;
using JasperSite.Models.Providers;
using JasperSite.Models.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace JasperSite.Test.Models.Database
{
    #region Tutorial
    /* 
     / Arrange
          List<Category> categories = new List<Category>() {
                new Category(){Id=1,Name="Nezařazeno"},
                 new Category(){Id=2,Name="Programování"},
                  new Category(){Id=3,Name="Zábava"},
            };
            IQueryable<Category> iqueryableList = categories.AsQueryable();
            Mock<DbSet<Category>> mockSetCategories = new Mock<DbSet<Category>>(); 
           
            mockSetCategories.As<IQueryable<Category>>().Setup(e => e.Provider).Returns(iqueryableList.Provider);
            mockSetCategories.As<IQueryable<Category>>().Setup(e => e.Expression).Returns(iqueryableList.Expression);
            mockSetCategories.As<IQueryable<Category>>().Setup(e => e.ElementType).Returns(iqueryableList.ElementType);
            mockSetCategories.As<IQueryable<Category>>().Setup(e => e.GetEnumerator()).Returns(iqueryableList.GetEnumerator());
                   

            Mock<IDatabaseContext> mockContext = new Mock<IDatabaseContext>();         
            mockContext.Setup(e=>e.Categories).Returns(mockSetCategories.Object);
            
            // Add
            mockSetCategories.Setup(d => d.Add(It.IsAny<Category>())).Callback<Category>(s => categories.Add(s));
            mockContext.Object.Categories.Add(new Category() { Name="new category 1"});
            int count_add = mockContext.Object.Categories.Count();

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

            DbHelper helper = new DbHelper(mockContext.Object);
         */
    #endregion
          

    [TestFixture]
    class DbHelperTest
    {
        /// <summary>
        /// This method automatically creates DbSet mock objects
        /// </summary>
        /// <typeparam name="T">Class to be mocked.</typeparam>
        /// <param name="elements">Underlying data source.</param>
        /// <returns></returns>
        private static Mock<DbSet<T>> CreateDbSetMock<T>(List<T> elements) where T : class
        {
            var elementsAsQueryable = elements.AsQueryable();
            var dbSetMock = new Mock<DbSet<T>>();

            dbSetMock.As<IQueryable<T>>().Setup(m => m.Provider).Returns(elementsAsQueryable.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.Expression).Returns(elementsAsQueryable.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(elementsAsQueryable.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(elementsAsQueryable.GetEnumerator());
            dbSetMock.Setup(d => d.Add(It.IsAny<T>())).Callback<T>(s => elements.Add(s));
            dbSetMock.Setup(d => d.Remove(It.IsAny<T>())).Callback<T>(s => elements.Remove(s));
            return dbSetMock;
        }

        [Test]
        public void DeleteCategory_IdExists_DeletesOnlyCategoryNotArticles()
        {            
            // Arrange categories
            List<Category> categories = new List<Category>() {
                new Category(){Id=1,Name="Nezařazeno"},
                 new Category(){Id=2,Name="Programování"},
                  new Category(){Id=3,Name="Škola"},
            };
            Mock<DbSet<Category>> mockSetCategories = CreateDbSetMock(categories);

            // Arrange articles
            List<Article> articles = new List<Article>() {
                new Article(){Id=1,Name="Nezařazeno - článek 1",CategoryId=1},
                 new Article(){Id=2,Name="Programování - článek 1",CategoryId=2},
                  new Article(){Id=3,Name="Škola - článek 1",CategoryId=3},
            };
            Mock<DbSet<Article>> mockSetArticles = CreateDbSetMock(articles);

            // Arrange dbContext
            Mock<IDatabaseContext> mockContext = new Mock<IDatabaseContext>();         
            mockContext.Setup(e=>e.Categories).Returns(mockSetCategories.Object);
            mockContext.Setup(e => e.Articles).Returns(mockSetArticles.Object);

            
            // Act - deleting first category
            DbHelper helper = new DbHelper(mockContext.Object);
            int idToBeDeleted = 2;
            helper.DeleteCategory(idToBeDeleted);

            // Assert
            int expectedNumberOfCategories = 2;
            int actualNumberOfCategores = mockContext.Object.Categories.Count();
            Assert.That(actualNumberOfCategores,Is.EqualTo(expectedNumberOfCategories));

            int expectedNumberOfArticles = 3;
            int actualNumberOfArticles = mockContext.Object.Articles.Count();
            Assert.That(actualNumberOfArticles, Is.EqualTo(expectedNumberOfArticles));

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
