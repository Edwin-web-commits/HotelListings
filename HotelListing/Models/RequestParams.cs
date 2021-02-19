using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

//implementing paging

namespace HotelListing.Models
{

    public class RequestParams
    {
        const int maxPageSize = 50;

        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;

        public int pageSize
        {
            get 
            { 
                return _pageSize; 
            }
            set
            {
                //if the requested value ( pageSize ) is greater than the maxPageSize, the just set the maxPageSize to 50 ,otherwise set the maxPageSize to that value
                _pageSize = (value > maxPageSize) ? maxPageSize : value; 
            }
        }
    }
}
