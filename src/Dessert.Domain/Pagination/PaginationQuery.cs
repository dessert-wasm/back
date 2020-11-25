using System;

namespace Dessert.Domain.Pagination
{
    public class PaginationQuery
    {
        public const uint MaxPageSize = 50;

        private uint _pageSize = MaxPageSize;
        private uint _pageNumber;

        public uint PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = Math.Max(value, 1);
        }

        public uint PageSize
        {
            get => _pageSize;
            set => _pageSize = Math.Min(value, MaxPageSize);
        }

        public uint ToSkip
            => PageSize * (PageNumber - 1);

        public bool IncludeCount { get; set; } = false;
    }
}