using JasperSiteCore.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Models.Database;

namespace JasperSiteCore.Areas.Admin.ViewModels
{
    public class ArticlesViewModel
    {
        public List<Article> Articles { get; set; }
        public int NumberOfArticles { get; set; }

        public int NumberOfCategories { get; set; }
        public bool UncategorizedCategoryExists { get; set; }
    }
}
