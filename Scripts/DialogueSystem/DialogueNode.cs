using UnityEngine;
using UnityEditor;
// using UnityEditorInternal;

[System.Serializable]
public struct DialogueNodeChild{
    public string triggerAnsPath;
    public DialogueNode child;
    public int extraFirendshipValue;
    public bool questAcceptTrigger;
    [HideInInspector]public int questID;
}

[CreateAssetMenu(fileName = "DialogueNode", menuName = "DialogueSystem/Dialogue Node", order = 0)]
public class DialogueNode : ScriptableObject {
    public DialogueNode parent;
    [TextArea(2,5)]
    public string content;
    [HideInInspector]public DialogueNodeChild[] childs;


    // #region Editor
    // [CustomEditor(typeof(DialogueNode))]
    // public class DialogueNodeEditor: Editor
    // {
    //     private ReorderableList list;
    //     private bool showChilds = true; 

    //     private void OnEnable()
    //     {
    //         list = new ReorderableList(serializedObject, serializedObject.FindProperty("childs"), true, true, true, true);

    //         list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => 
    //         {
    //             var element = list.serializedProperty.GetArrayElementAtIndex(index);
    //             rect.y += 2;
    //             float height = EditorGUIUtility.singleLineHeight;
    //             float padding = 2;
    //             float rightPadding = 2;
    //             GUIStyle boldLabelStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
    //             EditorGUI.LabelField(new Rect(rect.x,rect.y,rect.width - rightPadding, height), "Option " + index, boldLabelStyle);
    //             EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y + height + padding, rect.width - rightPadding, height), element.FindPropertyRelative("triggerAnsPath"));
    //             EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y + 2 * (height + padding), rect.width - rightPadding, height), element.FindPropertyRelative("child"));
    //             EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y + 3 * (height + padding), rect.width - rightPadding, height), element.FindPropertyRelative("extraFirendshipValue"));
    //             EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y + 4 * (height + padding), rect.width - rightPadding, height), element.FindPropertyRelative("questAcceptTrigger"));

    //             // Only draw the questID field if questAccept is true
    //             if (element.FindPropertyRelative("questAcceptTrigger").boolValue)
    //             {
    //                 EditorGUI.PropertyField(new Rect(rect.x + 8, rect.y + 5 * (height + padding), rect.width - rightPadding, height), element.FindPropertyRelative("questID"));
    //             }
    //         };

    //         list.elementHeightCallback = (int index) =>
    //         {
    //             var element = list.serializedProperty.GetArrayElementAtIndex(index);
    //             float height = EditorGUIUtility.singleLineHeight;
    //             float padding = 2;
    //             return element.FindPropertyRelative("questAcceptTrigger").boolValue ? 6 * (height + padding) : 5 * (height + padding);
    //         };

    //         list.drawHeaderCallback = (Rect rect) => {};
    //         list.headerHeight = 0;
    //     }

    //     public override void OnInspectorGUI()
    //     {
    //         base.OnInspectorGUI();

    //         serializedObject.Update();
    //         showChilds = EditorGUILayout.Foldout(showChilds, "Childs");
    //         if (showChilds)
    //         {
    //             list.DoLayoutList();
    //         }
    //         serializedObject.ApplyModifiedProperties();
    //     }
    // }
    // #endregion
}
