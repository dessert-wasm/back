using Dessert.Types;
using Dessert.Types.Arguments;
using HotChocolate.Types;

namespace Dessert.GraphQL
{
    public class MutationType : ObjectType<Mutation>
    {
        protected override void Configure(IObjectTypeDescriptor<Mutation> descriptor)
        {
            descriptor.BindFieldsExplicitly();

            descriptor
                .Field(t => t.Login(default, default, default, default))
                .Name("login")
                .Argument("email", arg => arg.Type<NonNullType<StringType>>())
                .Argument("password", arg => arg.Type<NonNullType<StringType>>())
                .Argument("remember", arg => arg.Type<NonNullType<BooleanType>>())
                .Type<NonNullType<AccountType>>();

            descriptor
                .Field(t => t.Register(default, default, default, default, default, default))
                .Name("register")
                .Argument("email", arg => arg.Type<NonNullType<StringType>>())
                .Argument("nickname", arg => arg.Type<NonNullType<StringType>>())
                .Argument("password", arg => arg.Type<NonNullType<StringType>>())
                .Type<NonNullType<AccountType>>();

            descriptor
                .Field(t => t.Logout(default))
                .Name("logout")
                .Authorize()
                .Type<NonNullType<BooleanType>>();

            descriptor
                .Field(t => t.CreateToken(default, default, default, default))
                .Authorize()
                .Name("createToken")
                .Description("Create a new token and return it's value")
                .Authorize()
                .Type<NonNullType<StringType>>()
                .Argument("description", x => x.Type<NonNullType<StringType>>());

            descriptor
                .Field(t => t.DeleteToken(default, default, default))
                .Name("deleteToken")
                .Description("Delete a token")
                .Type<NonNullType<BooleanType>>()
                .Argument("token", x => x.Type<StringType>());

            descriptor
                .Field(t => t.CreateModule(default, default, default, default, default, default))
                .Name("createModule")
                .Description("Create a new module")
                .Type<NonNullType<ModuleType>>()
                .Argument("token", x => x.Type<NonNullType<StringType>>())
                .Argument("name", x => x.Type<NonNullType<StringType>>())
                .Argument("description", x => x.Type<NonNullType<StringType>>())
                .Argument("replacements", x => x.Type<NonNullType<ListType<NonNullType<ModuleReplacementInputType>>>>())
                .Argument("isCore", x => x.Type<NonNullType<BooleanType>>());

            descriptor
                .Field(t => t.DeleteModule(default, default, default, default))
                .Name("deleteModule")
                .Description("Delete a module")
                .Type<NonNullType<BooleanType>>()
                .Argument("token", x => x.Type<NonNullType<StringType>>())
                .Argument("id", x => x.Type<NonNullType<IntType>>());

            descriptor
                .Field(t => t.UpdateUser(default, default))
                .Name("updateUser")
                .Description("Update a user")
                .Authorize()
                .Type<NonNullType<AccountType>>()
                .Argument("account", x => x.Type<NonNullType<UpdateUserInputType>>());
        }
    }
}
