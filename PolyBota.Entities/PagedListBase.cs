using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyBota.Entities
{
    public class PagedListBase<T> : PagedListSrcn
    {
        public List<T> DataLists { get; set; }
    }

    public class PagedListSrcn
    {
        public PagedListSrcn()
        {
            PageNumberList = new List<int>();
        }

        public List<int> PageNumberList { get; set; }
        public List<DropItem> PageSizeSelectList { get; set; }
        public int CurrentPage { get; set; }
        public int PageShowCount { get; set; }
    }
}
