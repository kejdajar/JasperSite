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
        
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string HtmlContent { get; set; }

        [Required]
        public DateTime PublishDate { get; set; }
      
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<Comment> Comments { get; set; }

        public ICollection<User> Users { get; set; }
    }

    public class Category
    {        
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Nickname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
    }

    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class Comment
    {
        public int Id { get; set; }
        public string HtmlContent { get; set; }
        public DateTime PublishDate { get; set; }        
       
    }
   
}
