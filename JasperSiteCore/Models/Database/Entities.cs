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
     

        public ICollection<User> Users { get; set; }
    }

    public class Category
    {        
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public partial class User
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

    public class Setting
    {
        [Key]
        public string Key { get; set; }

        public string Value { get; set; }
    }
      

    // Placeholders and Blocks

    public class Theme
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class BlockHolder
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("Theme")]
        public int ThemeId { get; set; }
        public Theme Theme { get; set; }

    }

    public class TextBlock
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
    }

    public class Holder_Block
    {
        public int Id { get; set; }

        [ForeignKey("BlockHolder")]
        public int BlockHolderId { get; set; }
        public BlockHolder BlockHolder { get; set; }

        [ForeignKey("TextBlock")]
        public int TextBlockId { get; set; }
        public TextBlock TextBlock { get; set; }

        public int Order { get; set; }
    }

    public class Image
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ImageData ImageData { get; set; }

        [ForeignKey("ImageData")]
        public int ImageDataId { get; set; }
    }

    public class ImageData
    {
        public int Id { get; set; }
        public byte[] Data { get; set; }
    }

}
