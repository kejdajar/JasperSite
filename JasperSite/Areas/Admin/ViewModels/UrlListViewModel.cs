using JasperSite.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using JasperSite.Models.Database;

namespace JasperSite.Areas.Admin.ViewModels
{
    public class UrlListViewModel
    {
        public List<string> Urls { get; set; }
        public int ArticleId { get; set; }
    }
}
