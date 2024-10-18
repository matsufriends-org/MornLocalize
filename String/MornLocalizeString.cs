using System;
using UnityEditor;
using UnityEngine;

namespace MornLocalize
{
    [Serializable]
    public class MornLocalizeString
    {
        [SerializeField] private MornLocalizeStringType _type = MornLocalizeStringType.Debug;
        [SerializeField] [Multiline] private string _debugString;
        [SerializeField] private string _key;

        public string Get(string language)
        {
            return _type switch
            {
                MornLocalizeStringType.Debug => _debugString,
                MornLocalizeStringType.FromKey => MornLocalizeGlobal.I.MasterData.Get(language, _key),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }

    [CustomPropertyDrawer(typeof(MornLocalizeString))]
    public class MornLocalizeStringDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            {
                // Initialize
                var height = EditorGUIUtility.singleLineHeight;
                var type = property.FindPropertyRelative("_type");
                var debugString = property.FindPropertyRelative("_debugString");
                var key = property.FindPropertyRelative("_key");

                // Rect
                position.height = height;
                EditorGUI.PropertyField(position, type, label);
                position.x += 10;
                position.width -= 10;
                var cachedLabelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 40;

                //typeによって描画するプロパティを変える
                switch ((MornLocalizeStringType)type.enumValueIndex)
                {
                    case MornLocalizeStringType.Debug:
                        var debugStringRect = new Rect(position.x, position.y + height, position.width, height * 3);
                        EditorGUI.PropertyField(debugStringRect, debugString, GUIContent.none);
                        break;
                    case MornLocalizeStringType.FromKey:
                        var keyRect = new Rect(position.x, position.y + height, position.width, height);
                        EditorGUI.PropertyField(keyRect, key, new GUIContent("Key"));
                        var valueRect = keyRect;
                        valueRect.y += height;
                        valueRect.height = height * 3;
                        var masterData = MornLocalizeGlobal.I.MasterData;
                        for (var i = 0; i < masterData.GetLanguageCount(); i++)
                        {
                            var language = masterData.GetLanguage(i);
                            GUI.enabled = false;
                            if (masterData.TryGet(language, key.stringValue, out var text))
                            {
                                EditorGUI.TextField(valueRect, language, text);
                            }
                            else
                            {
                                EditorGUI.TextField(valueRect, language, "Not Found");
                            }

                            GUI.enabled = true;
                            valueRect.y += height * 3;
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                EditorGUIUtility.labelWidth = cachedLabelWidth;
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUIUtility.singleLineHeight;
            var type = property.FindPropertyRelative("_type");
            switch ((MornLocalizeStringType)type.enumValueIndex)
            {
                case MornLocalizeStringType.Debug:
                    return height * 4;
                case MornLocalizeStringType.FromKey:
                    return height * 2 + height * 3 * MornLocalizeGlobal.I.MasterData.GetLanguageCount();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}