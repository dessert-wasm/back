using System;
using System.Collections.Generic;

namespace Dessert.Domain.Pagination
{
    public class PaginatedResult<T>
    {
        private uint _pageSize;

        public PaginatedResult() { }

        public PaginatedResult(PaginationQuery paginationQuery, int? totalRecords, IReadOnlyCollection<T> result)
        {
            Result = result;
            PageNumber = paginationQuery.PageNumber;
            _pageSize = paginationQuery.PageSize;
            TotalRecords = (uint?)totalRecords;
        }

        public IReadOnlyCollection<T> Result { get; set; }

        public uint PageNumber { get; set; }

        public uint PageSize
        {
            get
            {
                if (!TotalRecords.HasValue)
                    return _pageSize;
                return Math.Min(_pageSize, TotalRecords.Value - (_pageSize * (PageNumber - 1)));
            }
            set => _pageSize = value;
        }

        public uint? TotalRecords { get; set; }

        public uint? TotalPages
            => TotalRecords.HasValue ? (uint)Math.Ceiling(TotalRecords.Value / (double)_pageSize) : (uint?)null;
    }
}