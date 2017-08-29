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
        int SaveChanges();
    }

    public class DatabaseContext: DbContext,IDatabaseContext
    {
      

        public DatabaseContext(DbContextOptions<DatabaseContext> options):base(options)
        {

        }

        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<Category> Categories{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>().ToTable("Articles");
            modelBuilder.Entity<Category>().ToTable("Categories");
        }

       // public DbSet<JasperSiteCore.Areas.Admin.ViewModels.EditArticleViewModel> EditArticleViewModel { get; set; }
    }
}
