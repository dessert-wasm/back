using Dessert.Domain.Entities;
using Dessert.Types;
using Dessert.Types.Arguments;
using Dessert.Types.Pagination;
using Dessert.Utilities;
using HotChocolate.Types;

namespace Dessert.GraphQL
{
    public class QueryType : ObjectType<Query>
    {
        protected override void Configure(IObjectTypeDescriptor<Query> descriptor)
        {
            descriptor.BindFieldsExplicitly();

            descriptor.Description("Main entry point to retrieve data");

            descriptor
                .Field(t => t.Me(default, default))
                .Authorize()
                .Name("me")
                .Description("Get the current logged in user")
                .Type<NonNullType<AccountType>>();

            descriptor
                .Field(t => t.Search(default, default, default, default))
                .Name("search")
                .AddPaginationArgument()
                .Argument("query", x => x.Type<NonNullType<StringType>>())
                .Argument("type", x => x.Type<ModuleTypeEnumType>())
                .Type<NonNullType<PaginatedResultType<Module>>>();

            descriptor
                .Field(t => t.Module(default, default, default))
                .Name("module")
                .Type<NonNullType<ModuleType>>()
                .Argument("id", x => x.Type<NonNullType<IntType>>());

            descriptor
                .Field(t => t.User(default, default, default))
                .Name("user")
                .Type<NonNullType<AccountType>>()
                .Argument("id", x => x.Type<NonNullType<IntType>>());

            descriptor
                .Field(t => t.Tags(default, default))
                .Name("tags")
                .Type<NonNullType<ListType<NonNullType<ModuleTagType>>>>();

            descriptor
                .Field(t => t.Recommend(default, default, default))
                .Name("recommend")
                .Argument("dependencies",
                    a => a.Type<NonNullType<ListType<NonNullType<JSDependencyType>>>>())
                .Type<NonNullType<ListType<NonNullType<ModuleType>>>>();
        }
    }
}