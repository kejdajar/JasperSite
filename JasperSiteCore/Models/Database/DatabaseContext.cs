using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Areas.Admin.ViewModels;

namespace JasperSiteCore.Models.Database
{
    public interface IDatabaseContext
    {        
        DbSet<Article> Articles { get; set; }
        DbSet<Category> Categories { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<Setting> Settings { get; set; }        

        // Blocks
        DbSet<Theme> Themes { get; set; }
        DbSet<BlockHolder> BlockHolders { get; set; }
        DbSet<TextBlock> TextBlocks { get; set; }
        DbSet<Holder_Block> Holder_Block { get; set; }

        // Images
        DbSet<Image> Images { get; set; }
        DbSet<ImageData> ImageData { get; set; }

        int SaveChanges();
        
    }

    public class DatabaseContext: DbContext,IDatabaseContext
    {

        //public DatabaseContext(DbContextOptions<DatabaseContext> options):base(options)
        //{
        //}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           
                string typeOfDatabase = Configuration.GlobalWebsiteConfig.TypeOfDatabase;
                switch (typeOfDatabase)
                {
                    case "mssql": optionsBuilder.UseSqlServer(Configuration.GlobalWebsiteConfig.ConnectionString); break;
                    case "mysql": optionsBuilder.UseMySql(Configuration.GlobalWebsiteConfig.ConnectionString); break;
                    default: throw new NotSupportedDatabaseException();
                }
            
           
          
        }
       
        
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<Category> Categories{ get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }

        // Blocks
        public virtual DbSet<Theme> Themes { get; set; }
        public virtual DbSet<BlockHolder> BlockHolders { get; set; }
        public virtual  DbSet<TextBlock> TextBlocks { get; set; }
        public virtual DbSet<Holder_Block> Holder_Block { get; set; }

        //Images
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<ImageData> ImageData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>().ToTable("Articles");
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Setting>().ToTable("Settings");

            // Blocks
            modelBuilder.Entity<Theme>().ToTable("Themes");
            modelBuilder.Entity<BlockHolder>().ToTable("BlockHolders");
            modelBuilder.Entity<TextBlock>().ToTable("TextBlocks");
            modelBuilder.Entity<Holder_Block>().ToTable("HolderBlocks");

           // Images
            modelBuilder.Entity<Image>().ToTable("Images");
            modelBuilder.Entity<ImageData>().ToTable("ImageData");
        }

       // public DbSet<JasperSiteCore.Areas.Admin.ViewModels.EditArticleViewModel> EditArticleViewModel { get; set; }
    }
}
