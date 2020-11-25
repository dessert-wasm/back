using Dessert.Domain.Pagination;
using HotChocolate.Types;

namespace Dessert.Types.Pagination
{
    public class PaginationQueryType
        : InputObjectType<PaginationQuery>
    {
        protected override void Configure(IInputObjectTypeDescriptor<PaginationQuery> descriptor)
        {
            descriptor.BindFieldsExplicitly();

            descriptor.Field(t => t.PageNumber)
                .Description("Current page")
                .Type<NonNullType<IntType>>();

            descriptor.Field(t => t.PageSize)
                .Description($"Size of the page, max size: {PaginationQuery.MaxPageSize}")
                .Type<NonNullType<IntType>>();

            descriptor.Field(t => t.IncludeCount)
                .Description("Return the total number of elements")
                .Type<NonNullType<BooleanType>>();
        }
    }
}
