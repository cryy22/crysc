using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GulchGuardians.Editor
{
    public static class SerializedPropertyExtensions
    {
        public static SerializedProperty FindCSharpProperty(this SerializedObject obj, string propertyName)
        {
            return obj.FindProperty($"<{propertyName}>k__BackingField");
        }

        public static IEnumerable<T> GetElements<T>(this SerializedProperty property) where T : Object
        {
            for (var i = 0; i < property.arraySize; i++)
                yield return property.GetArrayElementAtIndex(i).objectReferenceValue as T;
        }

        public static void AddElementToArray(this SerializedProperty property, Object obj)
        {
            int newIndex = property.arraySize;
            property.InsertArrayElementAtIndex(newIndex);
            property.GetArrayElementAtIndex(newIndex).objectReferenceValue = obj;
        }

        public static void AddElementToArray<T>(this SerializedProperty property, T value) where T : struct
        {
            int newIndex = property.arraySize;
            property.InsertArrayElementAtIndex(newIndex);
            property.GetArrayElementAtIndex(newIndex).boxedValue = value;
        }

        public static int CountElementInArray(this SerializedProperty array, Object element)
        {
            return GetIndexesOfArrayElementInternal(array: array, element: element).Count();
        }

        public static int GetIndexOfArrayElement(this SerializedProperty array, Object element)
        {
            int[] indexes = GetIndexesOfArrayElementInternal(array: array, element: element).ToArray();
            if (indexes.Length > 0) return indexes[0];

            Debug.LogWarning($"element {element} not found in array property {array.name}");
            return -1;
        }

        private static IEnumerable<int> GetIndexesOfArrayElementInternal(SerializedProperty array, Object element)
        {
            if (!array.isArray)
            {
                Debug.LogWarning($"property {array.name} is not an array");
                yield break;
            }

            for (var i = 0; i < array.arraySize; i++)
            {
                SerializedProperty candidate = array.GetArrayElementAtIndex(i);
                if (candidate.objectReferenceValue == element) yield return i;
            }
        }
    }
}
