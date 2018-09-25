using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JasperSite.Models.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using JasperSite.Models;
using System.Text.Encodings.Web;



namespace JasperSite.Helpers
{
    



        /*--------------------Tag helpers---------------------------*/

        #region ArticleTagHelper
        [HtmlTargetElement("j-article")]
        //[RestrictChildren("j-name", "j-content")]
        public class JArticleTagHelper : TagHelper
        {
            // Dependency injection
            public JArticleTagHelper(DatabaseContext dbContext)
            {
                this.databaseContext = dbContext;
                this.databaseHelper = new DbHelper(dbContext);
            }

            private readonly DatabaseContext databaseContext;
            private readonly DbHelper databaseHelper;

            public int Id { get; set; }
            public override void Process(TagHelperContext context, TagHelperOutput output)
            {

                output.TagName = "div";
                Article a = databaseHelper.GetArticleById(Id);
                DataTransfer dataPackage = new DataTransfer() { ArticleId = Id, Article = a };

                context.Items.Add(typeof(JArticleTagHelper), dataPackage);
            }
        }

        [HtmlTargetElement("j-name", TagStructure = TagStructure.WithoutEndTag)]
        public class JNameTagHelper : TagHelper
        {
            public override void Process(TagHelperContext context, TagHelperOutput output)
            {
                DataTransfer data = (DataTransfer)context.Items[typeof(JArticleTagHelper)];
                output.TagName = "";
                output.Content.SetHtmlContent(HtmlEncoder.Default.Encode(data.Article.Name));

            }
        }

        [HtmlTargetElement("j-date", TagStructure = TagStructure.WithoutEndTag)]
        public class JDateTagHelper : TagHelper
        {
            public override void Process(TagHelperContext context, TagHelperOutput output)
            {
                DataTransfer data = (DataTransfer)context.Items[typeof(JArticleTagHelper)];
                output.TagName = "";
                output.Content.SetHtmlContent(HtmlEncoder.Default.Encode(data.Article.PublishDate.ToLongDateString() + ", " + data.Article.PublishDate.ToLongTimeString()));

            }
        }

        [HtmlTargetElement("j-content", TagStructure = TagStructure.WithoutEndTag)]
        public class JContentTagHelper : TagHelper
        {
            public override void Process(TagHelperContext context, TagHelperOutput output)
            {
                DataTransfer data = (DataTransfer)context.Items[typeof(JArticleTagHelper)];
                output.TagName = "";
                output.Content.SetHtmlContent(data.Article.HtmlContent);
            }
        }

        public class DataTransfer
        {
            public int ArticleId { get; set; }
            public Article Article { get; set; }
        }
        #endregion
    }

