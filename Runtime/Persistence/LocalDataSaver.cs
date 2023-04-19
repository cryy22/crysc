using UnityEngine;

namespace Crysc.Persistence
{
    public static class LocalDataSaver
    {
        public static void Save(string path, object data)
        {
            LocalStorage.Write(
                path: $"{path}.json",
                contents: JsonUtility.ToJson(data)
            );
        }

        public static T LoadNew<T>(string path)
        {
            return JsonUtility.FromJson<T>(
                LocalStorage.Read($"{path}.json")
            );
        }

        public static T LoadInto<T>(string path, T data) where T : class
        {
            JsonUtility.FromJsonOverwrite(
                json: LocalStorage.Read($"{path}.json"),
                objectToOverwrite: data
            );

            return data;
        }
    }
}
