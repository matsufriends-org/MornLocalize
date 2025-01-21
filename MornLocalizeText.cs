using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using VContainer;

namespace MornLocalize
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public sealed class MornLocalizeText : MonoBehaviour
    {
        [SerializeField] private MornLocalizeString _text;
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private List<MornLocalizeFontSet> _overrideFont;
        [Inject] private MornLocalizeCore _core;
        private string _cachedMaterialKey;

        private void Reset()
        {
            _label = GetComponent<TextMeshProUGUI>();
        }

        private void Awake()
        {
            _cachedMaterialKey = MornLocalizeGlobal.I.GetFontMaterialKey(_label.fontSharedMaterial);
            _core.OnLanguageChanged.Subscribe(UpdateText).AddTo(this);
            UpdateText(_core.CurrentLanguage);
        }

        private void UpdateText(string languageKey)
        {
            var defaultFont = MornLocalizeGlobal.I.GetDefaultFont(languageKey);
            var overrideFont = _overrideFont.Find(x => x.LanguageKey == languageKey);
            var font = overrideFont.Font ? overrideFont.Font : defaultFont;
            _label.font = font.Font;
            if (font.TryGetMaterial(_cachedMaterialKey, out var material))
            {
                _label.fontMaterial = material;
            }

            if (!string.IsNullOrEmpty(_text.GetKey()) && _text.GetStringType() == MornLocalizeStringType.FromKey)
            {
                _label.text = _text.Get(languageKey);
            }
        }
    }
}