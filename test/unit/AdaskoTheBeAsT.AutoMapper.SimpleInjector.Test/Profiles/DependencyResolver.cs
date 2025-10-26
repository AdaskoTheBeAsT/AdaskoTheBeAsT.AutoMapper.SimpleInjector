using AutoMapper;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;

public class DependencyResolver(ISomeService service)
    : IValueResolver<object, object, int>
{
    public int Resolve(object source, object destination, int destMember, ResolutionContext context)
    {
        return service.Modify(destMember);
    }
}
