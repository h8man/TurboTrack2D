using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace HQ
{
    // Tells Unity to use this Editor class with the WaveManager script component.
    [CustomEditor(typeof(TrackObject))]
    class TrackEditor : Editor
    {

        // This will contain the <wave> array of the WaveManager. 
        SerializedProperty Modifier;
        ReorderableList list;

        private void OnEnable()
        {
            //Gets the wave property in WaveManager so we can access it. 
            Modifier = serializedObject.FindProperty("Modifier");

            //Initialises the ReorderableList. We are creating a Reorderable List from the "wave" property. 
            //In this, we want a ReorderableList that is draggable, with a display header, with add and remove buttons        
            list = new ReorderableList(serializedObject, Modifier, true, true, true, true);

            list.drawElementCallback = DrawListItems;
            list.drawHeaderCallback = DrawHeader;
            list.elementHeight = 200;
        }
        void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index); //The element in the list

            // Create a property field and label field for each property. 
            //show = EditorGUI.BeginFoldoutHeaderGroup(new Rect(rect.x+50, rect.y, rect.width, rect.height), show, $"modifier {index}");
            int i = 0;
            //EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight), "Disabled");
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + i++ * EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("disabled"),
                new GUIContent("disabled")
            );
            
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + i++ * EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("label"),
                new GUIContent("label")
            );

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + i++ * EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("h"),
                new GUIContent("h")
            );

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + i++ * EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("curve"),
                new GUIContent("curve")
            );
            
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + i++ * EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("spriteX"),
                new GUIContent("sprite X")
            );

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + i++ * EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("flipX"),
                new GUIContent("flip X")
            );

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + i++ * EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("sprite"),
                new GUIContent("sprite")
            );
            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + i++ * EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("frequency"),
                new GUIContent("frequency")
            );

            EditorGUI.PropertyField(
                new Rect(rect.x, rect.y + i++ * EditorGUIUtility.singleLineHeight, rect.width, EditorGUIUtility.singleLineHeight),
                element.FindPropertyRelative("Segments"),
                new GUIContent("segments")
            );

            //EditorGUI.EndFoldoutHeaderGroup();
        }

        void DrawHeader(Rect rect)
        {
            string name = "Modifiers";
            EditorGUI.LabelField(rect, name);
        }

        //This is the function that makes the custom editor work
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            serializedObject.Update();
            Modifier.isExpanded = EditorGUILayout.Foldout(Modifier.isExpanded, "Modifiers");
            if (Modifier.isExpanded)
            {
                list.DoLayoutList();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
