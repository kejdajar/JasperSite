using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JasperSite.Models.Database;
using JasperSite.Models;
using JasperSite.Areas.Admin.Models;
using System.Text;
using Microsoft.AspNetCore.Html;

namespace JasperSite.Helpers
{
    public  class Components
    {

        public Components(IDatabaseContext dbContext, IJasperDataService dbHelper)
        {
            this._dbContext = dbContext;
            this._dbHelper = dbHelper;
        }

        private readonly IDatabaseContext _dbContext;
        private readonly IJasperDataService _dbHelper;

        /// <summary>
        /// Holder is container for any number of assigned bloks. Holder name is assigned to the active theme through CMS.
        /// Single page can contain more holders of the same name. Holder name must be beforehand registered in jasper.json theme file.
        /// </summary>
        /// <param name="holderName">Non-uniqe name of the registered holder.</param>
        /// <param name="dbContext">Database context from the dependecy injeciton.</param>
        /// <returns></returns>
        public HtmlString Holder(string holderName)
        {
            try
            {

                var holders = _dbHelper.GetAllBlockHolders();
                var holder_block = _dbHelper.GetAllHolder_Blocks();
                var blocks = _dbHelper.GetAllTextBlocks();
                var themes = _dbHelper.GetAllThemes();

                string currentThemeName = Configuration.GlobalWebsiteConfig.ThemeName;
                int currentThemeId = (from t in themes
                                      where t.Name == currentThemeName
                                      select t.Id).Single();

                var blocksToDisplay = (from h in holders
                                       from b in blocks
                                       from hb in holder_block
                                       where h.Name == holderName && hb.BlockHolderId == h.Id && b.Id == hb.TextBlockId && currentThemeId == h.ThemeId
                                       select new { BlockToDisplay = b, Order = hb.Order });

                StringBuilder sb = new StringBuilder();
                blocksToDisplay = blocksToDisplay.OrderBy(o => o.Order);
                foreach (var tb in blocksToDisplay)
                {
                    sb.Append(tb.BlockToDisplay.Content + "<hr/>");
                }
                return new HtmlString(sb.ToString());
            }
            catch
            {
                return new HtmlString(string.Empty);
            }
        }

       
    }

}
