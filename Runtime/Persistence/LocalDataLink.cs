namespace Crysc.Persistence
{
    public class LocalDataLink<TData> where TData : class
    {
        public delegate TData DefaultDataCreator();

        public TData Data => _data ??= LocalDataSaver.LoadInto(path: _path, data: _creator?.Invoke());

        private readonly string _path;
        private readonly DefaultDataCreator _creator;
        private TData _data;

        public LocalDataLink(string path, DefaultDataCreator creator)
        {
            _path = path;
            _creator = creator;
        }

        public void Save(TData data)
        {
            LocalDataSaver.Save(path: _path, data: data);
            _data = data;
        }

        public void Reset() { Save(_creator?.Invoke()); }
    }
}
