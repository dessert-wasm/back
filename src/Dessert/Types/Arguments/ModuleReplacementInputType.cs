using Dessert.Domain.Entities;
using HotChocolate.Types;

namespace Dessert.Types.Arguments
{
    public class ModuleReplacementInputType
        : InputObjectType<ModuleReplacement>
    {
        protected override void Configure(IInputObjectTypeDescriptor<ModuleReplacement> descriptor)
        {
            descriptor.BindFieldsExplicitly();

            descriptor.Field(f => f.Name)
                .Type<NonNullType<StringType>>();

            descriptor.Field(f => f.Link)
                .Type<NonNullType<UrlType>>();
        }
    }
}
