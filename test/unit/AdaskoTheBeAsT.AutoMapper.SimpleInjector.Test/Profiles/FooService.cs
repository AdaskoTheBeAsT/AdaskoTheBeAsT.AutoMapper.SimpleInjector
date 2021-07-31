namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles
{
    public class FooService : ISomeService
    {
        private readonly int _value;

        public FooService(int value)
        {
            _value = value;
        }

        public int Modify(int value) => value + _value;
    }
}
