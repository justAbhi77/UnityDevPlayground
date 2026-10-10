#if UNITY_EDITOR

using UnityEditor;

namespace Script.Editor
{
    public static class GenericCopyPaste
    {
        private static SerializedObject _source;

        [MenuItem("CONTEXT/Component/Copy Parameters via Name")]
        public static void CopySerialized(MenuCommand command)
        {
            _source = new SerializedObject(command.context);
        }

        [MenuItem("CONTEXT/Component/Paste Parameters via Name")]
        public static void PasteSerialized(MenuCommand command)
        {
            if (_source == null) return;

            // If types are identical, use Unity's built-in efficient copy
            if (_source.targetObject.GetType() == command.context.GetType())
            {
                EditorUtility.CopySerialized(_source.targetObject, command.context);
                return;
            }

            SerializedObject dest = new(command.context);
            var propIterator = _source.GetIterator();

            // Jump into the serialized object to skip the script type field
            if (propIterator.NextVisible(true))
            {
                while (propIterator.NextVisible(true))
                {
                    // Find the corresponding property in the destination
                    var propElement = dest.FindProperty(propIterator.name);

                    // Ensure the property exists in both and has the same type
                    if (propElement != null && propElement.propertyType == propIterator.propertyType)
                    {
                        // Copy the value/reference
                        dest.CopyFromSerializedProperty(propIterator);
                    }
                }
            }
            dest.ApplyModifiedProperties();
        }
    }
}
#endif