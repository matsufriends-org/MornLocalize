using System;
using UnityEditor;
using UnityEngine;

namespace MornLocalize
{
    [CustomPropertyDrawer(typeof(MornLocalizeSolver))]
    internal class MornLocalizeSolverPropertyDrawer : PropertyDrawer
    {
        private static float OneLineHeight => EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                var labelPosition = position;
                var keyPosition = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
                labelPosition.height = EditorGUIUtility.singleLineHeight;
                keyPosition.height = EditorGUIUtility.singleLineHeight;

                // keyを描画
                var keyProperty = property.FindPropertyRelative("key");
                keyProperty.stringValue = EditorGUI.TextField(keyPosition, keyProperty.stringValue);
                var key = keyProperty.stringValue;

                // 言語ごとの値を描画
                var master = MornLocalizeMasterData.Instance;
                var languageRect = new Rect(labelPosition.x, labelPosition.y + OneLineHeight, labelPosition.width, labelPosition.height);
                var valueRect = new Rect(keyPosition.x, keyPosition.y + OneLineHeight, keyPosition.width, keyPosition.height);
                foreach (var language in master.GetLanguages())
                {
                    // 言語名を描画
                    EditorGUI.indentLevel++;
                    EditorGUI.LabelField(languageRect, language);
                    EditorGUI.indentLevel--;
                    var cacheColor = GUI.contentColor;
                    string value;
                    int lineCount;

                    // 値を取得
                    if (master.TryGet(language, key, out value))
                    {
                        lineCount = value.Split('\n').Length;
                    }
                    else
                    {
                        GUI.contentColor = Color.red;
                        value = $"[{language}] [{keyProperty.stringValue}] not found";
                        lineCount = 1;
                    }

                    // 値の描画
                    valueRect.height = OneLineHeight * lineCount;
                    EditorGUI.LabelField(valueRect, value);
                    
                    // 次の行へ
                    valueRect.height = OneLineHeight;
                    languageRect.y += OneLineHeight * lineCount;
                    valueRect.y += OneLineHeight * lineCount;
                    GUI.contentColor = cacheColor;
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var key = property.FindPropertyRelative("key").stringValue;
            var master = MornLocalizeMasterData.Instance;
            var height = EditorGUIUtility.singleLineHeight;
            foreach (var language in master.GetLanguages())
            {
                var lineCount = master.TryGet(language, key, out var value) ? value.Split('\n').Length : 1;
                height += lineCount * EditorGUIUtility.singleLineHeight;
            }

            return height;
        }
    }
}