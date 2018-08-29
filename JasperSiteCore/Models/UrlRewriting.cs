using JasperSiteCore.Models.Database;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JasperSiteCore.Models
{
    public static class UrlRewriting
    {
        /// <summary>
        /// If the provided URL begins with "ArticleRoute" value from theme jasper.json, true will be returned, otherwise false.
        /// </summary>
        /// <param name="inputURL"></param>
        /// <returns></returns>
        /// <exception cref="InvalidUrlRewriteException"></exception>
        public static bool IsUrlRewriteRequest(string inputURL)
        {
            try
            {
                string articlesRoute = Configuration.WebsiteConfig.ArticleRoute;

                if (inputURL.StartsWith(articlesRoute))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                throw new InvalidUrlRewriteException(ex);
            }
        }


        /// <summary>
        /// Removes last slash from URL if present.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="InvalidUrlRewriteException"></exception>
        public static string CleanseUrl(string url)
        {
            try
            {
                if(url.EndsWith('/'))
                {
                    url = url.Remove(url.Length-1);
                }

                return url;
            }
            catch(Exception ex)
            {
                throw new InvalidUrlRewriteException("Url could not be cleansed.",ex);
            }
        }


        /// <summary>
        /// This method takes nice url, for instance /Home/Article/my_first_article and returns appropriate articleId from the database.
        /// In case of failure returns -1;
        /// </summary>
        /// <param name="inputURL"></param>
        /// <param name="dataService"></param>
        /// <returns></returns>
        public static int ReturnArticleIdFromNiceUrl(string inputURL, IJasperDataServicePublic dataService)
        {
            try
            {
                // mydomain.cz + /Home/Articles
                string articlesRoute = Configuration.WebsiteConfig.ArticleRoute;

                if (inputURL.StartsWith(articlesRoute))
                {
                    int ix = inputURL.IndexOf(articlesRoute);
                    if (ix != -1)
                    {
                        string requestedArticleUrl = inputURL.Substring(ix + articlesRoute.Length);

                        int articleId = dataService.Database.UrlRewrite.Where(ur => ur.Url == requestedArticleUrl).Select(s => s.ArticleId).Single();

                        //  string relativeUrl= inputURL.Replace(requestedArticleUrl, "?id=" + articleId);

                        return articleId;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
            }
            catch 
            {
                return -1;
            }
        }
    }
}
