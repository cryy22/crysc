using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Crysc.Persistence
{
    public static class LocalStorage
    {
        private const string _indexedDBDirectory = "/idbfs";
        private static string _dataPath;

        public static string Read(string path)
        {
            return File.ReadAllText(
                Path.Combine(path1: DataPath(), path2: path)
            );
        }

        public static void Write(string path, string contents)
        {
            string fullPath = Path.Combine(path1: DataPath(), path2: path);
            Directory.CreateDirectory(
                Path.GetDirectoryName(fullPath) ?? DataPath()
            );

            File.WriteAllText(path: fullPath, contents: contents);
            if (IsIndexedDB()) Crysc_SyncDB();
        }

        private static string DataPath()
        {
            return _dataPath ??= IsIndexedDB()
                ? Path.Combine(
                    path1: _indexedDBDirectory,
                    path2: Application.companyName,
                    path3: Application.productName
                )
                : Application.persistentDataPath;
        }

        [DllImport("__Internal")]
        private static extern void Crysc_SyncDB();

        private static bool IsIndexedDB()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
    }
}
