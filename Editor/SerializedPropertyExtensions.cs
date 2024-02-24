using UnityEditor;
using UnityEngine;

namespace GulchGuardians.Editor
{
    public static class SerializedPropertyExtensions
    {
        public static int GetIndexOfArrayElement(this SerializedProperty array, Object element)
        {
            if (!array.isArray)
            {
                Debug.LogWarning($"property {array.name} is not an array");
                return -1;
            }

            for (var i = 0; i < array.arraySize; i++)
            {
                SerializedProperty candidate = array.GetArrayElementAtIndex(i);
                if (candidate.objectReferenceValue == element) return i;
            }

            Debug.LogWarning($"element {element} not found in array property {array.name}");
            return -1;
        }
    }
}
