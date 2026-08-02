using AutoMapper;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;

internal sealed class DependencyPreCondition(ISomeService service)
    : IPreCondition<Source4, Dest4>
{
    public bool Evaluate(
        Source4 source,
        Dest4 destination,
        ResolutionContext context)
        => service.Modify(source.PreConditionValue) > 10;
}
