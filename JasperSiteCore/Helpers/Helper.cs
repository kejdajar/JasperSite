using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Models.Database;
using Microsoft.AspNetCore.Mvc.Rendering;
using JasperSiteCore.Models;
using System.Text.Encodings.Web;



namespace JasperSiteCore.Helpers
{
    public static class Helper
    {
        /// <summary>
        /// Transforms Theme-relative Url to Root-relative Url
        /// </summary>
        /// <param name="url">Theme-relative Url</param>
        /// <returns></returns>
        public static string JasperUrl(string url, bool addTilde = false)
        {
            // without slash on the beginning it would work only on pages without route parameters
            // urls has to look like: /Themes/Jasper/Styles/style.css
            // without the slash it would create the following: localhost/Home/Category/Themes/Jasper/Styles/style.css = undesirable
            string path = "/" + CustomRouting.RelativeThemePathToRootRelativePath(url);

            if (addTilde)
            {
                path = "~" + path;
            }
            return path;
        }
    }

    /*--------------------Tag helpers---------------------------*/

    #region ArticleTagHelper
    [HtmlTargetElement("j-article")]
    //[RestrictChildren("j-name", "j-content")]
    public class JArticleTagHelper : TagHelper
    {
        public int Id { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

            output.TagName = "div";
            Article a = DbHelper.GetArticleById(Id);
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
