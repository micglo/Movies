using System.Collections.Generic;

namespace Movies.Model.Common
{
    public class PagedItems
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public long TotalNumberOfPages { get; set; }
        public long TotalNumberOfRecords { get; set; }
        public string NextPageUrl { get; set; }
        public string PreviousPageUrl { get; set; }
        public IEnumerable<ICommonDto> Items { get; set; }
    }
}