using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JasperSiteCore.Models.Database;
using JasperSiteCore.Models;
using JasperSiteCore.Areas.Admin.Models;
using System.Text;
using Microsoft.AspNetCore.Html;

namespace JasperSiteCore.Helpers
{
    public static class J
    {
        public static HtmlString Holder(string holderName)
        {
            var holders = Configuration.DbHelper.GetAllBlockHolders();
            var holder_block = Configuration.DbHelper.GetAllHolder_Blocks();
            var blocks = Configuration.DbHelper.GetAllTextBlocks();
            var themes = Configuration.DbHelper.GetAllThemes(); 

            string currentThemeName = Configuration.GlobalWebsiteConfig.ThemeName;
            int currentThemeId = (from t in themes
                                     where t.Name == currentThemeName
                                     select t.Id).Single();

            var blocksToDisplay = (from h in holders
                                                         from b in blocks
                                                         from hb in holder_block                                                        
                                                         where h.Name == holderName && hb.BlockHolderId == h.Id && b.Id == hb.TextBlockId && currentThemeId == h.ThemeId
                                                         select new { BlockToDisplay=b,Order=hb.Order});

            StringBuilder sb = new StringBuilder();
            blocksToDisplay = blocksToDisplay.OrderBy(o => o.Order);
            foreach(var tb in blocksToDisplay)
            {
                sb.Append(tb.BlockToDisplay.Content+"<hr/>");
            }

            return new HtmlString(sb.ToString());

         //   BlockHolder holder = Configuration.DbHelper.GetAllBlockHolders().Where(b => b.Name == holderName).Single();
         // StringBuilder sb = new StringBuilder();

        }
    }
}
