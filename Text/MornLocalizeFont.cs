using UniRx;
using UnityEngine;
using VContainer;
using MornUGUI;

namespace MornLocalize
{
    [ExecuteAlways]
    [RequireComponent(typeof(MornUGUITextSetter))]
    public sealed class MornLocalizeFont : MonoBehaviour
    {
        [SerializeField, ReadOnly] private MornUGUITextSetter _setter;
        [SerializeField] private MornLocalizeFontSettings _settings;
        [Inject] private MornLocalizeCore _core;
        
        public MornLocalizeFontSettings Settings
        {
            get => _settings;
            set => _settings = value;
        }

        private void Awake()
        {
            if (Application.isPlaying)
            {
                _core.OnLanguageChanged.Subscribe(Adjust).AddTo(this);
                Adjust(_core.CurrentLanguage);
            }
        }

        private void Reset()
        {
            _setter = GetComponent<MornUGUITextSetter>();
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                var global = MornLocalizeGlobal.I;
                if (global == null)
                {
                    return;
                }

                Adjust(global.DebugLanguageKey);
            }
        }

        public void Adjust(string languageKey)
        {
            var global = MornLocalizeGlobal.I;
            if (global == null || _setter == null || _settings == null)
            {
                return;
            }

            var fontSettings = _settings.GetFontSettings(languageKey);
            if (fontSettings == null)
            {
                return;
            }

            var fontChanged = _setter.Text.font != fontSettings.Font;
            var sizeChanged = _setter.SizeSettings != _settings.SizeSettings;
            var materialChanged = _setter.MaterialType.Key != _settings.MaterialType.Key;
            var anyChanged = fontChanged || sizeChanged || materialChanged;
            if (anyChanged)
            {
                _setter.FontSettings = fontSettings;
                _setter.SizeSettings = _settings.SizeSettings;
                _setter.MaterialType.Key = _settings.MaterialType.Key;
                _setter.Adjust();
                global.SetDirty(_setter);
            }
        }
    }
}