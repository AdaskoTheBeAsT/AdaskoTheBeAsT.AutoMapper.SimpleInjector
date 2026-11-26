using AutoMapper;

namespace TestApp
{
    public class DependencyResolver : IValueResolver<Source2, Dest2, int>
    {
        private readonly ISomeService _service;

        public DependencyResolver(ISomeService service)
        {
            _service = service;
        }

        public int Resolve(Source2 source, Dest2 destination, int destMember, ResolutionContext context)
        {
            return _service.Modify(source.Value);
        }
    }
}
