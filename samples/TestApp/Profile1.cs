using AutoMapper;

namespace TestApp
{
    public class Profile1 : Profile
    {
        public Profile1()
        {
            CreateMap<Source, Dest>();
        }
    }
}
