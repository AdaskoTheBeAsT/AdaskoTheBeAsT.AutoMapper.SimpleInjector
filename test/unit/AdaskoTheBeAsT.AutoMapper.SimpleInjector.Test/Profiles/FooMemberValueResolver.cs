using AutoMapper;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;

internal sealed class FooMemberValueResolver : IMemberValueResolver<object, object, object, object>
{
    public object Resolve(object source, object destination, object sourceMember, object destMember, ResolutionContext context)
    {
#pragma warning disable CS8603 // Possible null reference return.
        return null;
#pragma warning restore CS8603 // Possible null reference return.
    }
}
