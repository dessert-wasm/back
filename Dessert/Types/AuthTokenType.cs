using Dessert.Domain.Entities;
using HotChocolate.Types;

namespace Dessert.Types
{
    public class AuthTokenType : ObjectType<AuthToken>
    {
        protected override void Configure(IObjectTypeDescriptor<AuthToken> descriptor)
        {
            descriptor.BindFieldsExplicitly();

            descriptor.Field(f => f.Id)
                .Type<NonNullType<IntType>>();

            descriptor.Field(f => f.Description)
                .Type<NonNullType<StringType>>();

            descriptor.Field(f => f.Token)
                .Type<NonNullType<StringType>>()
                .Resolver(ctx =>
                {
                    var authToken = ctx.Parent<AuthToken>();

                    return authToken.Token.Substring(0, 8);
                });
        }
    }
}