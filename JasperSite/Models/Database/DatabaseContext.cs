using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JasperSite.Areas.Admin.ViewModels;
using Microsoft.EntityFrameworkCore.Infrastructure;
using JasperSite.Models.Providers;

namespace JasperSite.Models.Database
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

        // Url rewrite
        DbSet<UrlRewrite> UrlRewrite { get; set; }

        int SaveChanges();    

    }

    public class DatabaseContext : DbContext, IDatabaseContext
    {
        public DatabaseContext()
        {
        }

        public DatabaseContext(IDatabaseContextOptions options) : base()
        {
            if (options == null) throw new ArgumentException();

            if(string.IsNullOrEmpty(options.ConnectionString) || string.IsNullOrEmpty(options.TypeOfDatabase) )
            {
                throw new ArgumentException();
            }

            this._connectionString = options.ConnectionString;
            this._typeOfDatabase = options.TypeOfDatabase;
        }

        private string _connectionString = string.Empty;
        private string _typeOfDatabase = string.Empty;
      

        /// <summary>
        /// This method is called on every HTTP request and also by .NET Command-line tools (e.g. $update-database).
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {            
            // if connection string was not passed manually in constructor
            if (string.IsNullOrEmpty(_connectionString) || string.IsNullOrEmpty(_typeOfDatabase))
            {
                // If Configuration.Initialize(); was not called yet, Configuration.GlobalWebsiteConfig is empty.
                // This happens when CLI tools are used for migrations -> in that case they will use 
                // settings from global jasper.json file.
                if (Configuration.GlobalWebsiteConfig == null)
                {
                    GlobalConfigDataProviderJson provider = new GlobalConfigDataProviderJson("jasper.json");
                    GlobalWebsiteConfig configForCliTools = new GlobalWebsiteConfig(provider);
                    this._connectionString = configForCliTools.ConnectionString;
                    this._typeOfDatabase = configForCliTools.TypeOfDatabase;                    
                }
                else
                {
                    this._connectionString = Configuration.GlobalWebsiteConfig.ConnectionString;
                    this._typeOfDatabase = Configuration.GlobalWebsiteConfig.TypeOfDatabase;
                }                
            }          

            switch (_typeOfDatabase)
            {
                case "mssql": optionsBuilder.UseSqlServer(_connectionString); break;
                case "mysql": optionsBuilder.UseMySql(_connectionString); break;
                default: throw new NotSupportedDatabaseException();
            }
           

        }


        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }

        // Blocks
        public virtual DbSet<Theme> Themes { get; set; }
        public virtual DbSet<BlockHolder> BlockHolders { get; set; }
        public virtual DbSet<TextBlock> TextBlocks { get; set; }
        public virtual DbSet<Holder_Block> Holder_Block { get; set; }

        //Images
        public virtual DbSet<Image> Images { get; set; }
        public virtual DbSet<ImageData> ImageData { get; set; }

        // URL rewrite
        public virtual DbSet<UrlRewrite> UrlRewrite { get; set; }

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

            // Url rewrite
            modelBuilder.Entity<UrlRewrite>().ToTable("UrlRewrite");
           
        }
    }

    /// <summary>
    /// Options for DatabaseContext class.
    /// </summary>
    public interface IDatabaseContextOptions
    {
        string ConnectionString { get; set; }
        string TypeOfDatabase { get; set; }
    }

}
