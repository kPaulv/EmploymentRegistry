using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.RequestFeatures
{
    public class PagedList<T> : List<T>
    {
        public MetaData MetaData { get; set; }

        public PagedList(List<T> dataList, int dataAmount, 
                                    int currentPage, int pageSize) 
        {
            MetaData = new MetaData
            {
                TotalCount = dataAmount,
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)dataAmount / pageSize)
            };

            AddRange(dataList);
        }

        public static PagedList<T> ToPagedList(IEnumerable<T> dataSource, 
                                                    int currentPage, int pageSize)
        {
            var dataAmount = dataSource.Count();
            var dataListForCurrentPage = dataSource.Skip((currentPage - 1) * pageSize)
                                                    .Take(pageSize).ToList();

            return new PagedList<T>(dataListForCurrentPage, dataAmount, 
                                                    currentPage, pageSize);
        }
    }
}
