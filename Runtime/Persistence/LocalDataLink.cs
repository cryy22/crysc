namespace Crysc.Persistence
{
    public class LocalDataLink<T> where T : class
    {
        public delegate T DefaultDataCreator();

        private readonly string _path;
        private readonly DefaultDataCreator _creator;
        private T _data;

        public LocalDataLink(string path, DefaultDataCreator creator)
        {
            _path = path;
            _creator = creator;
        }

        public T Data => _data ??= LocalDataSaver.LoadInto(path: _path, data: _creator?.Invoke());

        public void Save(T data)
        {
            LocalDataSaver.Save(path: _path, data: data);
            _data = data;
        }
    }
}
