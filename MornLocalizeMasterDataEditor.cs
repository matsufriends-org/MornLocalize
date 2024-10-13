#if UNITY_EDITOR
using System;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace MornLocalize
{
    [CustomEditor(typeof(MornLocalizeMasterData))]
    internal sealed class MornLocalizeMasterDataEditor : Editor
    {
        private bool _isLoading;
        private Vector2 _scrollPosition;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var masterData = (MornLocalizeMasterData)target;
            if (GUILayout.Button("Load"))
            {
                LoadAsync().Forget();
            }

            using (var scrollView = new EditorGUILayout.ScrollViewScope(_scrollPosition))
            {
                _scrollPosition = scrollView.scrollPosition;
                DrawTable(masterData);
            }
        }

        private async UniTask LoadAsync()
        {
            if (_isLoading)
            {
                MornLocalizeLogger.LogWarning("Load is already running!");
                return;
            }

            var masterData = (MornLocalizeMasterData)target;
            MornLocalizeLogger.Log("Load Start!");
            _isLoading = true;
            try
            {
                await masterData.LoadAsync();
                EditorUtility.SetDirty(masterData);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            _isLoading = false;
            MornLocalizeLogger.Log("Load End!");
        }

        private static void DrawTable(MornLocalizeMasterData masterData)
        {
            if (!masterData.IsValid)
            {
                return;
            }

            var languages = masterData.GetLanguages();
            var totalWidth = EditorGUIUtility.currentViewWidth - languages.Length * 25;
            var width = totalWidth / (languages.Length + 1);

            // Header
            DrawRow(() =>
            {
                GUILayout.Label("key", GUILayout.Width(width));
                foreach (var language in languages)
                {
                    VerticalSeparator();
                    GUILayout.Label(language, GUILayout.Width(width));
                }
            });
            HorizontalSeparator();

            // Contents
            foreach (var key in masterData.GetKeys())
            {
                DrawRow(() =>
                {
                    GUILayout.Label(key, GUILayout.Width(width));
                    foreach (var language in languages)
                    {
                        VerticalSeparator();
                        var value = masterData.Get(language, key);
                        GUILayout.Label(value, GUILayout.Width(width));
                    }
                });
                HorizontalSeparator();
            }
        }

        private static void HorizontalSeparator()
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
            }
        }

        private static void VerticalSeparator()
        {
            using (new GUILayout.VerticalScope())
            {
                GUILayout.Box("", GUILayout.Width(1), GUILayout.ExpandHeight(true));
            }
        }

        private static void DrawRow(Action action)
        {
            using (new GUILayout.HorizontalScope())
            {
                action();
            }
        }
    }
}
#endif