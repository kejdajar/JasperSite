using JasperSite.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JasperSite.Models.Database;

namespace JasperSite.Areas.Admin.ViewModels
{
    public class ArticlesViewModel
    {
        public List<Article> Articles { get; set; }
        public int NumberOfArticles { get; set; }

        public int NumberOfCategories { get; set; }
        public bool UncategorizedCategoryExists { get; set; }

        public ArticleListViewModel ArticleListModel { get; set; }
    }
}
