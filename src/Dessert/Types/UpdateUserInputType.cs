using Dessert.Domain.Entities.Identity;
using HotChocolate.Types;

namespace Dessert.Types
{
    public class UpdateUserInputType
        : InputObjectType<Account>
    {
        protected override void Configure(IInputObjectTypeDescriptor<Account> descriptor)
        {
            descriptor.BindFieldsExplicitly();

            descriptor.Field(t => t.Nickname)
                .Type<NonNullType<StringType>>();

            descriptor.Field(t => t.ProfilePicUrl)
                .Type<NonNullType<StringType>>();

        }
    }
}
