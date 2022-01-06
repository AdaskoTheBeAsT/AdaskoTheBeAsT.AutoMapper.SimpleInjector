using AutoMapper;

namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;

public class Profile1 : Profile
{
    public Profile1()
    {
        CreateMap<Source, Dest>();
    }
}
