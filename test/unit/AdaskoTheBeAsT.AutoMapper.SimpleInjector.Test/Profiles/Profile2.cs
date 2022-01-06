using AutoMapper;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;

internal sealed class Profile2 : Profile
{
    public Profile2()
    {
        CreateMap<Source2, Dest2>()
            .ForMember(d => d.ResolvedValue, opt => opt.MapFrom<DependencyResolver>())
            .ForMember(d => d.ConvertedValue, opt => opt.ConvertUsing<DependencyValueConverter, int>());
    }
}
