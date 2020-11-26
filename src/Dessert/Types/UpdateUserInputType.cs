using Dessert.Domain.Entities.Identity;
using HotChocolate.Types;

namespace Dessert.Types
{
    public class UpdateUserInputType
        : InputObjectType<ApplicationUser>
    {
        protected override void Configure(IInputObjectTypeDescriptor<ApplicationUser> descriptor)
        {
            descriptor.BindFieldsExplicitly();

            descriptor.Name("AccountInput");

            descriptor.Field(t => t.Nickname)
                .Type<NonNullType<StringType>>();

            descriptor.Field(t => t.ProfilePicUrl)
                .Type<NonNullType<StringType>>();

        }
    }
}
