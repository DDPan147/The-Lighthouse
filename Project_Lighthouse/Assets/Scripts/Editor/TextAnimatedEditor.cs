using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TextAnimated))]
public class TextAnimatedEditor : UnityEditor.UI.TextEditor
{
    private SerializedProperty baseMaterialProp;

    protected override void OnEnable()
    {
        base.OnEnable();
        this.baseMaterialProp = serializedObject.FindProperty("baseMaterial");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        EditorGUILayout.PropertyField(this.baseMaterialProp, new GUIContent("Base Material"));
        serializedObject.ApplyModifiedProperties();
    }
}
