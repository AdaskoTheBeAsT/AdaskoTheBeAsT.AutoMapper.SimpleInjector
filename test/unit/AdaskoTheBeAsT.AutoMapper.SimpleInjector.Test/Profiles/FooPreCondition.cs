using AutoMapper;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;

internal sealed class FooPreCondition : IPreCondition<object, object>
{
    public bool Evaluate(
        object source,
        object destination,
        ResolutionContext context)
        => true;
}
