namespace Crysc.Registries
{
    public interface IRegistrar<out T>
    {
        public T Registrant { get; }
    }
}
