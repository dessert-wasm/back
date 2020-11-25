using Dessert.Domain.Pagination;
using HotChocolate.Types;

namespace Dessert.Types.Pagination
{
    public class PaginatedResultType<T> : ObjectType<PaginatedResult<T>>
    {
        protected override void Configure(IObjectTypeDescriptor<PaginatedResult<T>> descriptor)
        {
            descriptor.BindFieldsExplicitly();

            descriptor.Field(f => f.PageNumber)
                .Type<NonNullType<IntType>>();

            descriptor.Field(f => f.PageSize)
                .Type<NonNullType<IntType>>();

            descriptor.Field(f => f.TotalRecords)
                .Type<IntType>();

            descriptor.Field(f => f.TotalPages)
                .Type<IntType>();

            descriptor.Field(f => f.Result);
        }
    }
}