using Dessert.DataLoaders;
using Dessert.Domain.Entities;
using GreenDonut;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace Dessert.Types
{
    public class ModuleType : ObjectType<Module>
    {
        protected override void Configure(IObjectTypeDescriptor<Module> descriptor)
        {
            descriptor.BindFieldsExplicitly();

            descriptor.Field(f => f.Id)
                .Type<NonNullType<IntType>>();

            descriptor.Field(f => f.Name)
                .Type<NonNullType<StringType>>();

            descriptor.Field(f => f.Description)
                .Type<NonNullType<StringType>>();
                
            descriptor.Field(f => f.GithubLink)
                .Type<UrlType>();

            descriptor.Field(f => f.PublishedDateTime)
                .Type<NonNullType<DateTimeType>>();

            descriptor.Field(f => f.LastUpdatedDateTime)
                .Type<NonNullType<DateTimeType>>();

            descriptor.Field(f => f.IsCore)
                .Type<NonNullType<BooleanType>>();

            descriptor.Field("author")
                .Type<NonNullType<AccountType>>()
                .Resolver(ctx =>
                {
                    var dataContext = ctx.DataLoader<AccountById>();
                    var module = ctx.Parent<Module>();

                    return dataContext.LoadAsync(module.AuthorId);
                });

            descriptor.Field("tags")
                .Type<NonNullType<ListType<NonNullType<ModuleTagType>>>>()
                .Resolver(ctx =>
                {
                    var dataContext = ctx.DataLoader<ModuleTagsByModuleId>();
                    var module = ctx.Parent<Module>();

                    return dataContext.LoadAsync(module.Id);
                });

            descriptor.Field("replacements")
                .Type<NonNullType<ListType<NonNullType<ModuleReplacementType>>>>()
                .Resolver(ctx =>
                {
                    var dataContext = ctx.DataLoader<ModuleReplacementByModuleId>();
                    var module = ctx.Parent<Module>();

                    return dataContext.LoadAsync(module.Id);
                });
        }
    }
}
