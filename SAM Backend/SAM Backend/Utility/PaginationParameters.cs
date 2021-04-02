using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.Utility
{
    public class PaginationParameters
    {
        const int MaxPageSize = 20;
        private int _PageNumber { get; set; } = 1;
        private int _PageSize { get; set; } = 10;
        public int PageSize
        {
            get
            {
                return _PageSize;
            }
            set
            {
                _PageSize = (value <= MaxPageSize) ? value : MaxPageSize;
            }
        }
        public int PageNumber
        {
            get
            {
                return _PageNumber;
            }
            set
            {
                _PageNumber = (value >= 1) ? value : 1;
            }
        }
    }
}
