using UnityEngine;

namespace MornLocalize.Button
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public sealed class MornLocalizeButton : MonoBehaviour
    {
        [SerializeField, ReadOnly] private MornLocalizeText[] _texts;
        [SerializeField, ReadOnly] private MornLocalizeFont[] _fonts;
        [SerializeField] private MornLocalizeString _text;
        [SerializeField] private MornLocalizeFontSettings _fontSettings;

        [ContextMenu("Rebuild")]
        private void Reset()
        {
            _texts = GetComponentsInChildren<MornLocalizeText>();
            _fonts = GetComponentsInChildren<MornLocalizeFont>();
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                Adjust();
            }
        }

        private void Adjust()
        {
            var global = MornLocalizeGlobal.I;
            if (global == null)
            {
                return;
            }

            if (_text != null)
            {
                foreach (var text in _texts)
                {
                    if (!text.Text.IsEqual(_text))
                    {
                        text.Text = _text;
                        text.Adjust(global.DebugLanguageKey);
                        global.SetDirty(text);
                    }
                }
            }

            if (_fontSettings != null)
            {
                foreach (var font in _fonts)
                {
                    if (font.Settings != _fontSettings)
                    {
                        font.Settings = _fontSettings;
                        font.Adjust(global.DebugLanguageKey);
                        global.SetDirty(font);
                    }
                }
            }
        }
    }
}