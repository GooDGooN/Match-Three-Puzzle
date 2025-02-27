using System;
using UnityEditor;
using UnityEngine;

namespace Coffee.UISoftMask
{
    [CustomEditor(typeof(SoftMaskable), true)]
    [CanEditMultipleObjects]
    public class SoftMaskableEditor : AutoGeneratedEditor
    {
        private SerializedProperty _ignoreSelf;
        private SerializedProperty _ignoreChildren;
        private SerializedProperty _power;

        private void OnEnable()
        {
            _ignoreSelf = serializedObject.FindProperty("m_IgnoreSelf");
            _ignoreChildren = serializedObject.FindProperty("m_IgnoreChildren");
            _power = serializedObject.FindProperty("m_Power");
        }

        public override void OnInspectorGUI()
        {
            if (0 < (target.hideFlags & HideFlags.DontSave))
            {
                base.OnInspectorGUI();
            }

            serializedObject.Update();
            DrawProperty(_ignoreSelf, x => x.SetMaterialDirty());
            DrawProperty(_ignoreChildren, x => x.SetMaterialDirtyForChildren());
            DrawProperty(_power, x => x.SetMaterialDirty());
            serializedObject.ApplyModifiedProperties();

            if (targets.Length == 1 && target is SoftMaskable softMaskable && softMaskable.ignored)
            {
                var message = softMaskable.ignoreSelf
                    ? "This graphic will be not soft masked."
                    : "This graphic will be not soft masked by parents.";
                EditorGUILayout.HelpBox(message, MessageType.Warning);
            }
        }

        private void DrawProperty(SerializedProperty sp, Action<SoftMaskable> onChanged)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(sp);
            if (!EditorGUI.EndChangeCheck()) return;

            foreach (var t in targets)
            {
                onChanged(t as SoftMaskable);
            }
        }
    }
}
