using AutoMapper;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;

internal sealed class DependencyCondition(ISomeService service)
    : ICondition<Source4, Dest4, int>
{
    public bool Evaluate(
        Source4 source,
        Dest4 destination,
        int sourceMember,
        int destMember,
        ResolutionContext context)
        => service.Modify(sourceMember) > 10;
}
