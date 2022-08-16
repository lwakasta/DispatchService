using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DispatchService.Models
{
    public class PaginationModel
    {
        public List<RequestViewModel> Requests { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int RecordCount { get; set; }
        public int PageTotal { get; set; }
    }
}