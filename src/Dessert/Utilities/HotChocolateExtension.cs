using System;
using System.Security.Claims;
using Dessert.Domain.Pagination;
using Dessert.Types.Pagination;
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

        public static string GetEmail(this ClaimsPrincipal claimsPrincipal)
        {
            var fromEmail = claimsPrincipal.FindFirst(ClaimTypes.Email);
            if (fromEmail != null)
                return fromEmail.Value;
            var fromName = claimsPrincipal.FindFirst(ClaimTypes.Name);
            if (fromName != null)
                return fromName.Value;
            throw new Exception("you got no claim :/");
        }
    }
}