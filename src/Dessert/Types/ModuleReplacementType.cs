using Dessert.Domain.Entities;
using HotChocolate.Types;

namespace Dessert.Types
{
    public class ModuleReplacementType : ObjectType<ModuleReplacement>
    {
        protected override void Configure(IObjectTypeDescriptor<ModuleReplacement> descriptor)
        {
            descriptor.BindFieldsExplicitly();

            descriptor.Field(f => f.Id)
                .Type<NonNullType<IntType>>();

            descriptor.Field(f => f.Name)
                .Type<NonNullType<StringType>>();

            descriptor.Field(f => f.Link)
                .Type<UrlType>();
        }
    }
}