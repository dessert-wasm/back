using System.Security.Claims;
using Dessert.Types.Pagination;
using Dessert.Utilities.Pagination;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace Dessert.Utilities
{
    public static class HotChocolateExtension
    {
        public static ClaimsPrincipal GetClaimsPrincipal(this IResolverContext context)
        {
            return context.ContextData["ClaimsPrincipal"] as ClaimsPrincipal;
        }

        private const string PaginationArgumentName = "pagination";

        public static IObjectFieldDescriptor AddPaginationArgument(
            this IObjectFieldDescriptor descriptor)
        {
            return descriptor
                .Argument(PaginationArgumentName, a => a.Type<NonNullType<PaginationQueryType>>());
        }

        public static PaginationQuery GetPaginationQuery(this IResolverContext context)
        {
            return context.Argument<PaginationQuery>(PaginationArgumentName);
        }

    }
}