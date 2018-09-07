using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JasperSite.Helpers
{
    public class JasperPaging<T>
    {
        public JasperPaging(List<T> fullDataCollection, int currentPageNumber, int itemsPerPage)
        {
            if (fullDataCollection == null) throw new ArgumentNullException();
            this.FullDataCollection = fullDataCollection;
            this.CurrentPageNumber = currentPageNumber;           
            this.ItemsPerPage = itemsPerPage;

            this.NumberOfItems = FullDataCollection.Count();

            double divide = Convert.ToDouble(NumberOfItems) / Convert.ToDouble(ItemsPerPage);
            this.NumberOfPagesNeeded = Convert.ToInt32(Math.Ceiling(divide));

            // Check nagative and too big page numbers
            if (CurrentPageNumber > NumberOfPagesNeeded) CurrentPageNumber = NumberOfPagesNeeded;
            if (CurrentPageNumber < 1) CurrentPageNumber = 1;
        }
            
        public List<T> FullDataCollection { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPageNumber { get; set; }


        public int NumberOfItems { get; }
        public int NumberOfPagesNeeded { get; }

        public List<T> GetCurrentPageItems()
        {   
            List<T> list = (FullDataCollection).Skip(ItemsPerPage * (CurrentPageNumber - 1)).Take(ItemsPerPage).ToList();            
            return list;
        }
        
    }
}
