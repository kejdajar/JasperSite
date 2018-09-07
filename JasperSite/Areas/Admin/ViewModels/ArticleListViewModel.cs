using JasperSite.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JasperSite.Areas.Admin.ViewModels
{
    public class ArticleListViewModel
    {
        public int CurrentPage { get; set; }
        public int TotalNumberOfPages { get; set; }
        public List<Article> Articles { get; set; }
       
    }
}
