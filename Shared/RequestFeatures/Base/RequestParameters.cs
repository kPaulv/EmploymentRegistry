﻿namespace Shared.RequestFeatures.Base
{
    public abstract class RequestParameters
    {
        const int MAX_PAGE_SIZE = 50;
        public int PageNumber { get; set; } = 1;

        private int _pageSize = 10;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                _pageSize = value > MAX_PAGE_SIZE ? MAX_PAGE_SIZE : value;
            }
        }

        // specify the order field and the order type(asc/desc)
        public string? OrderBy { get; set; }

        // specify the fields for Data Shaping
        public string? Fields { get; set; }
    }
}
