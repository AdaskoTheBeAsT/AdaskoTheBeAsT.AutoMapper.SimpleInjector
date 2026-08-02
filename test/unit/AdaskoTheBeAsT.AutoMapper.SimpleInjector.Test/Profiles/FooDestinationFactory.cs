using AutoMapper;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;

internal sealed class FooDestinationFactory : IDestinationFactory<object, object>
{
    public object Construct(object source, ResolutionContext context) => new();
}
