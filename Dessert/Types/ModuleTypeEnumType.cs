using Dessert.Domain.Enums;
using HotChocolate.Types;

namespace Dessert.Types
{
    public class ModuleTypeEnumType : EnumType<ModuleTypeEnum>
    {
        protected override void Configure(IEnumTypeDescriptor<ModuleTypeEnum> descriptor)
        {
            descriptor.BindValuesImplicitly();
        }
    }
}