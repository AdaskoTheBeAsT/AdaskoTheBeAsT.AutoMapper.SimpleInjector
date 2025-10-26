namespace AdaskoTheBeAsT.AutoMapper.SimpleInjector.Test.Profiles;

public class FooService(int firstValue)
    : ISomeService
{
    public int Modify(int value) => value + firstValue;
}
