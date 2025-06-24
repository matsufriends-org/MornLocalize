#if UNITY_EDITOR
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace MornLocalize
{
    [CustomEditor(typeof(MornLocalizeSettings))]
    internal sealed class MornLocalizeSettingsEditor : Editor
    {
        private bool _isLoading;
        private Vector2 _scrollPosition;

        public override void OnInspectorGUI()
        {
            var settings = (MornLocalizeSettings)target;
            if (GUILayout.Button("URLを開く"))
            {
                settings.Open();
            }

            if (GUILayout.Button("シート更新 & Keyを登録する"))
            {
                MornLocalizeDownloader.UpdateWithProgressAsync(settings, true, true).Forget();
            }

            if (GUILayout.Button("シート更新"))
            {
                MornLocalizeDownloader.UpdateWithProgressAsync(settings, true, false).Forget();
            }

            if (GUILayout.Button("Keyを登録する"))
            {
                MornLocalizeDownloader.UpdateWithProgressAsync(settings, false, true).Forget();
            }

            base.OnInspectorGUI();
        }
    }
}
#endif