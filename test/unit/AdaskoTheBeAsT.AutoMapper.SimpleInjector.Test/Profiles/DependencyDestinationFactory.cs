using AutoMapper;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;

internal sealed class DependencyDestinationFactory(ISomeService service)
    : IDestinationFactory<Source4, Dest4>
{
    public Dest4 Construct(Source4 source, ResolutionContext context)
        => new() { ConstructedValue = service.Modify(1) };
}
