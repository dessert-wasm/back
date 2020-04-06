using Dessert.Domain.Entities;
using HotChocolate.Types;

namespace Dessert.Types
{
    public class ModuleTagType : ObjectType<ModuleTag>
    {
        protected override void Configure(IObjectTypeDescriptor<ModuleTag> descriptor)
        {
            descriptor.BindFieldsExplicitly();

            descriptor.Field(f => f.Id)
                .Type<NonNullType<IntType>>();

            descriptor.Field(f => f.Name)
                .Type<NonNullType<StringType>>();
        }
    }
}