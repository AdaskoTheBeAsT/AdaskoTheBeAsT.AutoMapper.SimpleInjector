using AutoMapper;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;

internal sealed class DependencyValueConverter : IValueConverter<int, int>
{
    private readonly ISomeService _service;

    public DependencyValueConverter(ISomeService service) => _service = service;

    public int Convert(int sourceMember, ResolutionContext context)
        => _service.Modify(sourceMember);
}
