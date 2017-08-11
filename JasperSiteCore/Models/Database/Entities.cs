using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace JasperSiteCore.Models.Database
{
    public class Article
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string HtmlContent { get; set; }

        [Required]
        public DateTime PublishDate { get; set; }
      //  public List<Category> Categories { get; set; }
    }

    public class Category
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
    }
   
}
