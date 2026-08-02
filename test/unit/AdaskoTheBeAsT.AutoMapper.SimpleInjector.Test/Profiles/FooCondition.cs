using AutoMapper;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;

internal sealed class FooCondition : ICondition<object, object, object>
{
    public bool Evaluate(
        object source,
        object destination,
        object sourceMember,
        object destMember,
        ResolutionContext context)
        => true;
}
