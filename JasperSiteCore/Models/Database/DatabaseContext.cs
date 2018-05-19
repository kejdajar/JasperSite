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
            switch(typeOfDatabase)
            {
                case "mssql": optionsBuilder.UseSqlServer(Configuration.GlobalWebsiteConfig.ConnectionString); break;
                case "mysql": optionsBuilder.UseMySQL(Configuration.GlobalWebsiteConfig.ConnectionString); break;
                default: throw new NotSupportedDatabaseException();
            }            
          
        }

        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<Category> Categories{ get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>().ToTable("Articles");
            modelBuilder.Entity<Category>().ToTable("Categories");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<User>().ToTable("Users");
        }

       // public DbSet<JasperSiteCore.Areas.Admin.ViewModels.EditArticleViewModel> EditArticleViewModel { get; set; }
    }
}
