using AutoMapper;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;

internal sealed class DependencyValueConverter(ISomeService service)
    : IValueConverter<int, int>
{
    public int Convert(int sourceMember, ResolutionContext context)
        => service.Modify(sourceMember);
}
