using HotChocolate.Types;

namespace Dessert.Types.Arguments
{
    public class JSDependency
    {
        public string Name { get; set; }
    }
    
    public class JSDependencyType
        : InputObjectType<JSDependency>
    {
        protected override void Configure(IInputObjectTypeDescriptor<JSDependency> descriptor)
        {
            descriptor.BindFieldsExplicitly();

            descriptor.Field(t => t.Name)
                .Description("Dependency name")
                .Type<NonNullType<StringType>>();
        }
    }
}