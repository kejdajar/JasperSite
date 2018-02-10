﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace JasperSiteCore.Models.Database
{
    public class DbHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="database"></param>
        /// <exception cref="DatabaseContextNullException"></exception>
        public DbHelper(IDatabaseContext database)
        {
            this._db = database ?? throw new DatabaseContextNullException();
        }

        private IDatabaseContext _db;

        public  List<Article> GetAllArticles()
        {           
            if (_db.Articles.Any())
            {
                return _db.Articles.ToList();
            }
           else
            {
                return null;
            }
        }

        public List<Article> GetAllArticles(int categoryId)
        {
            if (_db.Articles.Any())
            {
                return _db.Articles.Where(a=>a.CategoryId==categoryId).ToList();
            }
            else
            {
                return null;
            }
        }

        public  Article GetArticleById(int id)
        {
            IDatabaseContext database = _db;

            if (database.Articles.Any())
            {
                return database.Articles.Where(a => a.Id==id).Single();
            }
            else
            {
                return null;
            }
        }

        public  int AddArticle()
        {
            IDatabaseContext database = _db;
            JasperSiteCore.Models.Database.Article articleEntity = new Article()
            {               
                HtmlContent = "Váš článek začíná zde...",
                Name = "Nový článek",
                PublishDate = DateTime.Now
            };
            database.Articles.Add(articleEntity);
            database.SaveChanges();
            return articleEntity.Id;

        }

        public void EditArticle(JasperSiteCore.Areas.Admin.ViewModels.EditArticleViewModel articleViewModel)
        {
            IDatabaseContext database = _db;
            Article oldArticleToChange = database.Articles.Where(a => a.Id == articleViewModel.Id).Single();


            oldArticleToChange.HtmlContent = articleViewModel.HtmlContent;
            oldArticleToChange.Name = articleViewModel.Name;
            oldArticleToChange.PublishDate = articleViewModel.PublishDate;
            oldArticleToChange.CategoryId = articleViewModel.SelectedCategoryId;

           // database.Articles.Add(oldArticleToChange);
            database.SaveChanges();
        }


        public void DeleteArticle(int articleId)
        {
            IDatabaseContext database = _db;
            Article articleToDelete = database.Articles.Where(a => a.Id == articleId).Single();
            database.Articles.Remove(articleToDelete);
            database.SaveChanges();
        }



        public  List<Category> GetAllCategories()
        {
            IDatabaseContext database = _db;
            if(database.Categories.Any())
            {
                return database.Categories.ToList();
            }
            else
            {
                return null;
            }
        }

        public void AddNewCategory(string categoryName)
        {
            IDatabaseContext database = _db;
            database.Categories.Add(new Category() { Name = categoryName });
            database.SaveChanges();
        }

        public void DeleteCategory(int categoryId)
        {
            IDatabaseContext database = _db;
            Category goner = database.Categories.Where(c => c.Id == categoryId).Single();
            database.Categories.Remove(goner);
            database.SaveChanges();
        }


        public int GetNumberOfEntities(string nameOfContextProperty)
        {
            IDatabaseContext database = _db;
            if (database.Categories.Any())
            {
                object i = database.GetType().GetProperty(nameOfContextProperty).GetValue(database);
                IEnumerable<Article> a = i as IEnumerable<Article>;
                return a.Count();
            }
            else
            {
                return 0;
            }
        }
    }
}
