using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace JasperSiteCore.Areas.Admin.ViewModels
{
    public class EditArticleViewModel
    {
       
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string HtmlContent { get; set; }

        [Required]
        public DateTime PublishDate { get; set; }
    }
}
