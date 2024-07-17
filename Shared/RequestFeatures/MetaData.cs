using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.RequestFeatures
{
    public class MetaData
    {
        // number of the current viewed page
        public int CurrentPage { get; set; }

        // total amount of pages
        public int TotalPages { get; set; }

        // amount of data items per one page
        public int PageSize { get; set; }

        // total amount of data in repo
        public int TotalCount { get; set; }

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

    }
}
