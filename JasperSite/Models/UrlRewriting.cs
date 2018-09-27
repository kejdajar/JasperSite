using JasperSite.Models.Database;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JasperSite.Models
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
                string articlesRoute = UrlRewriting.NormalizeUrl(Configuration.WebsiteConfig.ArticleRoute);
                inputURL = UrlRewriting.NormalizeUrl(inputURL);

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
        /// Removes last slash from URL if present. Adds slash to the begginning of the URL if absent.
        /// Additionally, the URL is translated to lowercase.
        /// E.g. : Home/Articles/ --> /home/articles
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <exception cref="InvalidUrlRewriteException"></exception>
        public static string NormalizeUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            try
            {
                if(url.EndsWith('/'))
                {
                    url = url.Remove(url.Length-1);
                }

                if (!url.StartsWith('/'))
                {
                    url = "/" + url;
                }
                
                return url.ToLower();
            }
            catch(Exception ex)
            {
                throw new InvalidUrlRewriteException("Url could not be cleansed.",ex);
            }
        }

        /// <summary>
        /// Normalizes and then compares URLs.
        /// </summary>
        /// <param name="url1"></param>
        /// <param name="url2"></param>
        /// <returns></returns>
        /// <exception cref="InvalidUrlRewriteException"></exception>
        public static bool CompareUrls(string url1, string url2)
        {
            try
            {
                string normalized1 = NormalizeUrl(url1);
                string normalized2 = NormalizeUrl(url2);
                if (normalized1 == normalized2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidUrlRewriteException(ex);              
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
                inputURL = UrlRewriting.NormalizeUrl(inputURL);

                // mydomain.cz + /Home/Articles
                string articlesRoute =UrlRewriting.NormalizeUrl(Configuration.WebsiteConfig.ArticleRoute);

                if (inputURL.StartsWith(articlesRoute))
                {
                    int ix = inputURL.IndexOf(articlesRoute);
                    if (ix != -1)
                    {
                        string requestedArticleUrl = inputURL.Substring(ix + articlesRoute.Length);

                        // database stores URL without slashes:
                        requestedArticleUrl = requestedArticleUrl.Replace("/", "");

                        requestedArticleUrl = requestedArticleUrl.Replace("%20", " "); // in case custom URL contains space (in DB is space not saved as %20)
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
