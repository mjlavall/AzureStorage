using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AzureStorage
{
    class ListViewColumnSorter : IComparer
    {
        public int SortColumn { get; set; }
        public SortOrder Order { get; set; }
        private CaseInsensitiveComparer ObjectCompare;

        public ListViewColumnSorter()
        {
            SortColumn = 0;
            Order = SortOrder.None;
            ObjectCompare = new CaseInsensitiveComparer();
        }

        public int Compare(object x, object y)
        {
            var listviewX = x as ListViewItem ?? new ListViewItem();
            var listviewY = y as ListViewItem ?? new ListViewItem();
            var xVal = listviewX.SubItems[SortColumn].Text;
            var yVal = listviewY.SubItems[SortColumn].Text;
            if (SortColumn == 1) // size comparison
            {
                List<string> units = new List<string>{"B", "KB", "MB", "GB", "TB"};
                var unitX = xVal.Substring(xVal.Length - 2);
                var unitY = yVal.Substring(yVal.Length - 2);
                if (xVal == "< 1KB") unitX = "B";
                if (yVal == "< 1KB") unitY = "B";
                var precedenceX = units.IndexOf(unitX);
                var precedenceY = units.IndexOf(unitY);
                xVal = precedenceX + xVal;
                yVal = precedenceY + yVal;
            }
            if (SortColumn == 2) // date comparison
            {
                var xDate = DateTime.MinValue;
                var yDate = DateTime.MinValue;
                DateTime.TryParse(xVal, out xDate);
                DateTime.TryParse(yVal, out yDate);
                xVal = (xDate > yDate ? "1" : "0") + xVal;
                yVal = (yDate > xDate ? "1" : "0") + yVal;
            }
            int compareResult = ObjectCompare.Compare(xVal, yVal);
            if (Order == SortOrder.Ascending) return compareResult;
            if (Order == SortOrder.Descending) return (-compareResult);
            return 0;
        }
    }
}
