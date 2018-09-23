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
            mockContext.Setup(e => e.Categories).Returns(mockSetCategories.Object);
            mockContext.Setup(e => e.Articles).Returns(mockSetArticles.Object);

            // Act - deleting first category
            DbHelper helper = new DbHelper(mockContext.Object);
            int idToBeDeleted = 2;
            helper.DeleteCategory(idToBeDeleted);

            // Assert
            int expectedNumberOfCategories = 2;
            int actualNumberOfCategores = mockContext.Object.Categories.Count();
            Assert.That(actualNumberOfCategores, Is.EqualTo(expectedNumberOfCategories));

            int expectedNumberOfArticles = 3;
            int actualNumberOfArticles = mockContext.Object.Articles.Count();
            Assert.That(actualNumberOfArticles, Is.EqualTo(expectedNumberOfArticles));
        }


    }
}
